using System;
using System.Data;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class ProductsForm : Form
    {
        private readonly SQLiteDatabase db;
        private readonly DataGridView dataGridViewProducts = new DataGridView();

        public ProductsForm(SQLiteDatabase database)
        {
            InitializeComponent();
            db = database;
            InitializeProductsGrid();
            LoadProducts();
        }

        private void InitializeProductsGrid()
        {
            dataGridViewProducts.Dock = DockStyle.Fill;
            dataGridViewProducts.AllowUserToAddRows = false;
            dataGridViewProducts.AllowUserToDeleteRows = false;
            dataGridViewProducts.ReadOnly = true;
            Controls.Add(dataGridViewProducts);
        }

        private void LoadProducts()
        {
            try
            {
                var products = db.GetDataTable("SELECT * FROM Products");
                dataGridViewProducts.DataSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }
    }
}