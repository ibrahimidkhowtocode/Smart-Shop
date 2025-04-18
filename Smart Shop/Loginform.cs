using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Smart_Shop
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both username and password.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!SQLiteDatabase.ValidateUser(txtUsername.Text, txtPassword.Text))
            {
                MessageBox.Show("Invalid username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check for low stock
            var lowStock = SQLiteDatabase.GetLowStockProducts();
            if (lowStock.Count > 0)
            {
                MessageBox.Show($"Low stock alert for: {string.Join(", ", lowStock)}",
                    "Inventory Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (txtUsername.Text == "admin")
            {
                this.Hide();
                new AdminForm().Show();
            }
            else
            {
                // Show cashier notes form
                var notesForm = new CashierNotesForm();
                if (notesForm.ShowDialog() == DialogResult.OK)
                {
                    SQLiteDatabase.SaveCashierNote(txtUsername.Text, notesForm.Notes);
                }

                this.Hide();
                new CashierForm(txtUsername.Text).Show();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = '*';
        }
    }
}