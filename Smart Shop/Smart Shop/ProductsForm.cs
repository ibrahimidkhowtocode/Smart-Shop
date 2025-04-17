using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class ProductsForm : Form
    {
        private readonly SQLiteDatabase db;
        private DataGridView dataGridViewProducts;

        public ProductsForm()
        {
            InitializeComponent();
            db = new SQLiteDatabase("SmartShop.db");
            InitializeProductsGrid();
            LoadProducts();
        }

        private void InitializeProductsGrid()
        {
            dataGridViewProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                Name = "dataGridViewProducts"
            };
            Controls.Add(dataGridViewProducts);
        }

        private void LoadProducts()
        {
            dataGridViewProducts.DataSource = db.GetDataTable("SELECT * FROM products");
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Add implementation
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Edit implementation
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            // Delete implementation
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            // Export implementation
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            // Import implementation
        }
    }
}