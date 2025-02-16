using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CurrencyXChange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            btnConvert.Click += btnConvert_Click;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            List<string> currencies = await GetCurrencyListAsync();
            if (currencies.Count > 0) {
                // populate "fromCurrency" combox
                cmbFromCurrency.DataSource = new List<string>(currencies);
                // populate "toCurrency" combox
                cmbToCurrency.DataSource = new List<string>(currencies);
                // set default selections
                cmbFromCurrency.SelectedItem = "USD";
                cmbToCurrency.SelectedItem = "EUR";
            }
        }

        private async Task<List<string>> GetCurrencyListAsync()
        {
            string apiKey = "a822183107ba204e7328ff09"; // api key
            // api endpoint to get the list of supported currencies
            string apiURL = $"https://v6.exchangerate-api.com/v6/{apiKey}/codes";

            using (HttpClient client = new HttpClient())
            {
                // Send an asynchronous get request to the api
                HttpResponseMessage response = await client.GetAsync(apiURL);
                if (response.IsSuccessStatusCode)
                {
                    // parse json response
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                    // check if api response is successful
                    if (jsonData.result == "success")
                    {
                        // return list of currency codes
                        List<string> currencies = new List<string>();
                        foreach (var currency in jsonData.supported_codes)
                        {
                            //extract currency code
                            currencies.Add(currency[0].ToString());
                        }
                        return currencies;
                    }
                    else
                    {
                        // handle error in api response
                        MessageBox.Show("Failed to load currency list from API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return new List<string>();
                    }
                }
                else
                {
                    // handle http request failure
                    MessageBox.Show("Failed to connect to the API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<string>();
                }
            }
        }

        private async Task<decimal> GetConversionRateAsync(string fromCurrency, string toCurrency)
        {
            string apiKey = "a822183107ba204e7328ff09";
            string apiUrl = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/{fromCurrency}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                    if (jsonData.result == "success")
                    {
                        // retrieve conversion rate for the "toCurrency"
                        decimal conversionRate = jsonData.conversion_rates[toCurrency];
                        return conversionRate;
                    }
                    else
                    {
                        MessageBox.Show("Failed to get conversion rate from API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return 0m;
                    }
                }
                else
                {
                    MessageBox.Show("Failed to connect to the API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0m;
                }
            }
        }

        private async void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                // get input amount
                decimal amount = decimal.Parse(txtAmount.Text);
                // get selected currencies
                string fromCurrency = cmbFromCurrency.SelectedItem.ToString();
                string toCurrency = cmbToCurrency.SelectedItem.ToString();
                // get conversion rate from the api
                decimal conversionRate = await GetConversionRateAsync(fromCurrency, toCurrency);

                if (conversionRate > 0)
                {
                    // convert
                    decimal result = amount * conversionRate;

                    // display result
                    txtResult.Text = result.ToString("F2");
                }
                else
                {
                    txtResult.Text = "Error";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid input. Please check the amount and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
