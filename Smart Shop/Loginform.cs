using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class LoginForm : Form
    {
        private readonly SQLiteDatabase db;

        public LoginForm()
        {
            InitializeComponent();
            db = new SQLiteDatabase();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password");
                return;
            }

            try
            {
                var query = "SELECT Role FROM Users WHERE Username = @username AND Password = @password";
                var result = db.GetDataTable(query,
                    new SQLiteParameter("@username", username),
                    new SQLiteParameter("@password", password));

                if (result.Rows.Count == 0)
                {
                    MessageBox.Show("Invalid username or password");
                    return;
                }

                string role = result.Rows[0]["Role"].ToString();
                Form nextForm = role switch
                {
                    "Admin" => new AdminForm(),
                    "Cashier" => new CashierForm(username),
                    _ => throw new Exception("Unknown role")
                };

                Hide();
                nextForm.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
            }
        }
    }
}