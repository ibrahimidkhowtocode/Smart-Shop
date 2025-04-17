using System;
using System.Drawing;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class AddEditProductForm : Form
    {
        public string ItemName { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int StockQuantity { get; set; }
        public int MinStockLevel { get; set; } = 5;

        public AddEditProductForm(bool isEditMode = false)
        {
            InitializeComponent();
            if (isEditMode) this.Text = "Edit Product";

            // Configure numeric controls
            numPurchasePrice.DecimalPlaces = 2;
            numPurchasePrice.Minimum = 0.01m;
            numPurchasePrice.Increment = 0.01m;

            numSalePrice.DecimalPlaces = 2;
            numSalePrice.Minimum = 0.01m;
            numSalePrice.Increment = 0.01m;

            numQuantity.Minimum = 0;
            numMinStock.Minimum = 1;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                ItemName = txtName.Text;
                Barcode = txtBarcode.Text;
                PurchasePrice = numPurchasePrice.Value;
                SalePrice = numSalePrice.Value;
                StockQuantity = (int)numQuantity.Value;
                MinStockLevel = (int)numMinStock.Value;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private static void ShowError(string message, Control control)
        {
            MessageBox.Show(message, "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control.Focus();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("Product name is required", txtName);
                return false;
            }

            if (numPurchasePrice.Value >= numSalePrice.Value)
            {
                ShowError("Sale price must be higher than purchase price", numSalePrice);
                return false;
            }

            if (numQuantity.Value < 0)
            {
                ShowError("Quantity cannot be negative", numQuantity);
                return false;
            }

            return true;
        }
    }
}