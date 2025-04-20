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

            try
            {
                if (productId.HasValue)
                {
                    SQLiteDatabase.ExecuteNonQuery(
                        @"UPDATE Products SET 
                            Name=@name, 
                            Barcode=@barcode, 
                            Price=@price, 
                            Cost=@cost, 
                            Quantity=@quantity 
                        WHERE Id=@id",
                        ("@name", txtName.Text),
                        ("@barcode", txtBarcode.Text),
                        ("@price", numPrice.Value),
                        ("@cost", numCost.Value),
                        ("@quantity", (int)numQuantity.Value),
                        ("@id", productId.Value)
                    );
                }
                else
                {
                    SQLiteDatabase.ExecuteNonQuery(
                        @"INSERT INTO Products (Name, Barcode, Price, Cost, Quantity) 
                         VALUES (@name, @barcode, @price, @cost, @quantity)",
                        ("@name", txtName.Text),
                        ("@barcode", txtBarcode.Text),
                        ("@price", numPrice.Value),
                        ("@cost", numCost.Value),
                        ("@quantity", (int)numQuantity.Value)
                    );
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}