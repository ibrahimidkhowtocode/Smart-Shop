using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class CashierForm : Form
    {
        private readonly SQLiteDatabase db;
        private readonly string cashierUsername;

        public CashierForm(string username)
        {
            InitializeComponent();
            db = new SQLiteDatabase();
            cashierUsername = username;
            InitializeUI();
        }

        private void InitializeUI()
        {
            // Your existing UI initialization code
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            // Your existing add to cart logic
            db.LogHistory("AddToCart", $"Product added by {cashierUsername}");
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            // Your existing checkout logic
            db.LogHistory("Checkout", $"Sale completed by {cashierUsername}");
        }
    }
}