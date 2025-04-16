using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace StoreManagementSystem
{
    public partial class AdminForm : Form
    {
        public AdminForm()
        {
            InitializeComponent();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            lblWelcome.Text = "Welcome, Admin! Today is " + DateTime.Now.ToString("dddd, MMMM dd");
        }

        private void dashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void productsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("We'll add products here later!");
        }

        // Add similar methods for other menu items
    }
}