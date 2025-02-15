using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CurrencyXChange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> currencies = new List<string>
            {
                "USD", // US Dollar
                "EUR", // Euro
                "GBP", // British Pound
                "INR", // Indian Rupee
                "AUD", // Australian Dollar
                "CAD", // Canadian Dollar
                "JPY", // Japanese Yen
                "CNY"  // Chinese Yuan
            };

            // populate "fromCurrency" combox
            cmbFromCurrency.DataSource = new List<string>(currencies);

            // populate "toCurrency" combox
            cmbToCurrency.DataSource = new List<string>(currencies);

            // set default selections
            cmbFromCurrency.SelectedItem = "USD";
            cmbToCurrency.SelectedItem = "EUR";
        }
    }
}
