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
            db = new SQLiteDatabase("SmartShop.db");
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "admin" && password == "admin123")
            {
                new AdminForm().Show();
                this.Hide();
            }
            else
            {
                var cashier = db.GetDataTable(
                    "SELECT * FROM cashiers WHERE username=@user AND password=@pass",
                    new SQLiteParameter("@user", username),
                    new SQLiteParameter("@pass", password)
                );

                if (cashier.Rows.Count > 0)
                {
                    new CashierForm(username).Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid login!");
                }
            }
        }
    }
}