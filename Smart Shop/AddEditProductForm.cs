using System;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class AddEditProductForm : Form
    {
        private int? productId;

        public AddEditProductForm() : this(null, "", 0m, 0m, 0, "") { }

        public AddEditProductForm(int? id, string name, decimal price, decimal cost, int quantity, string barcode)
        {
            InitializeComponent();
            productId = id;
            txtName.Text = name;
            numPrice.Value = price;
            numCost.Value = cost;
            numQuantity.Value = quantity;
            txtBarcode.Text = barcode;

            if (productId.HasValue)
            {
                this.Text = "Edit Product";
            }
            else
            {
                this.Text = "Add New Product";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter product name", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numPrice.Value <= 0 || numCost.Value <= 0)
            {
                MessageBox.Show("Please enter valid price and cost", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (productId.HasValue)
            {
                // Update existing product
                SQLiteDatabase.ExecuteNonQuery(
                    @"UPDATE Products SET Name=@name, Price=@price, Cost=@cost, 
                     Quantity=@quantity, Barcode=@barcode WHERE Id=@id",
                    ("@name", txtName.Text),
                    ("@price", numPrice.Value),
                    ("@cost", numCost.Value),
                    ("@quantity", (int)numQuantity.Value),
                    ("@barcode", txtBarcode.Text),
                    ("@id", productId.Value)
                );
            }
            else
            {
                // Add new product
                SQLiteDatabase.ExecuteNonQuery(
                    @"INSERT INTO Products (Name, Price, Cost, Quantity, Barcode) 
                     VALUES (@name, @price, @cost, @quantity, @barcode)",
                    ("@name", txtName.Text),
                    ("@price", numPrice.Value),
                    ("@cost", numCost.Value),
                    ("@quantity", (int)numQuantity.Value),
                    ("@barcode", txtBarcode.Text)
                );
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