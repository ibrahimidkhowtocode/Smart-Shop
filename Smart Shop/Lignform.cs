using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace StoreManagementSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            CreateDatabase(); // We'll create this next
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            if (username == "admin" && password == "admin123")
            {
                AdminForm adminForm = new AdminForm();
                adminForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Wrong username or password");
            }
        }

        private void CreateDatabase()
        {
            if (!System.IO.File.Exists("StoreDB.db"))
            {
                SQLiteConnection.CreateFile("StoreDB.db");

                using (var connection = new SQLiteConnection("Data Source=StoreDB.db"))
                {
                    connection.Open();

                    string createUsers = @"CREATE TABLE Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Password TEXT NOT NULL,
                        Role TEXT NOT NULL)";

                    new SQLiteCommand(createUsers, connection).ExecuteNonQuery();

                    // Add admin user
                    string addAdmin = @"INSERT INTO Users (Username, Password, Role) 
                                      VALUES ('admin', 'admin123', 'Admin')";
                    new SQLiteCommand(addAdmin, connection).ExecuteNonQuery();
                }
            }
        }
    }
}