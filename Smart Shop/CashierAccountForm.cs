using System;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class CashierAccountForm : Form
    {
        private string? username;

        public CashierAccountForm(string? existingUsername = null)
        {
            InitializeComponent();
            username = existingUsername;

            if (!string.IsNullOrEmpty(username))
            {
                this.Text = "Edit Cashier Account";
                var cashier = SQLiteDatabase.GetUser(username);
                if (cashier != null)
                {
                    txtUsername.Text = cashier.Username;
                    txtPassword.Text = cashier.Password;
                    cmbWageType.Text = cashier.WageType;
                    numWageAmount.Value = cashier.WageAmount;
                }
            }
            else
            {
                this.Text = "Create Cashier Account";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter username and password", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(cmbWageType.Text) || numWageAmount.Value <= 0)
            {
                MessageBox.Show("Please select wage type and amount", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(username))
            {
                // Create new cashier
                SQLiteDatabase.CreateUser(txtUsername.Text, txtPassword.Text, "cashier",
                    cmbWageType.Text, numWageAmount.Value);
            }
            else
            {
                // Update existing cashier
                SQLiteDatabase.UpdateUser(username, txtPassword.Text,
                    cmbWageType.Text, numWageAmount.Value);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}