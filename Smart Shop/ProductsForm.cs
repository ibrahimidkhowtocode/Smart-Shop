using System;
using System.Windows.Forms;
using System.Data;

namespace Smart_Shop
{
    public partial class ProductsForm : Form
    {
        private DataGridView dataGridViewProducts = new DataGridView();
        private Button btnAdd = new Button();
        private Button btnEdit = new Button();
        private Button btnBack = new Button();

        public ProductsForm()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadProducts();
        }

        private void InitializeDataGridView()
        {
            dataGridViewProducts.Dock = DockStyle.Top;
            dataGridViewProducts.Height = 300;
            dataGridViewProducts.ReadOnly = true;
            dataGridViewProducts.AllowUserToAddRows = false;
            dataGridViewProducts.AllowUserToDeleteRows = false;
            dataGridViewProducts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            btnAdd.Text = "Add Product";
            btnAdd.Top = 310;
            btnAdd.Width = 100;

            btnEdit.Text = "Edit Product";
            btnEdit.Top = 310;
            btnEdit.Left = 110;
            btnEdit.Width = 100;

            btnBack.Text = "Back";
            btnBack.Top = 310;
            btnBack.Left = 220;
            btnBack.Width = 100;

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnBack.Click += BtnBack_Click;

            this.Controls.Add(dataGridViewProducts);
            this.Controls.Add(btnAdd);
            this.Controls.Add(btnEdit);
            this.Controls.Add(btnBack);
        }

        private void LoadProducts()
        {
            dataGridViewProducts.DataSource = SQLiteDatabase.GetProducts();
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var addForm = new AddEditProductForm();
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dataGridViewProducts.SelectedRows.Count > 0 &&
                dataGridViewProducts.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                var editForm = new AddEditProductForm(
                    id: Convert.ToInt32(row["Id"]),
                    name: row["Name"]?.ToString() ?? string.Empty,
                    price: Convert.ToDecimal(row["Price"]),
                    cost: Convert.ToDecimal(row["Cost"]),
                    quantity: Convert.ToInt32(row["Quantity"]),
                    barcode: row["Barcode"]?.ToString() ?? string.Empty
                );
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                }
            }
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}