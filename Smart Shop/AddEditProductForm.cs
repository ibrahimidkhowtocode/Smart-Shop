using System;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class AddEditProductForm : Form
    {
        public string Barcode { get; private set; } = string.Empty;
        public string ItemName { get; private set; } = string.Empty;
        public decimal PurchasePrice { get; private set; }
        public decimal SalePrice { get; private set; }
        public int StockQuantity { get; private set; }
        public int MinStockLevel { get; private set; }

        public AddEditProductForm()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            Barcode = txtBarcode.Text;
            ItemName = txtName.Text;
            PurchasePrice = numPurchasePrice.Value;
            SalePrice = numSalePrice.Value;
            StockQuantity = (int)numQuantity.Value;
            MinStockLevel = (int)numMinStock.Value;

            DialogResult = DialogResult.OK;
            Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                ShowError("Barcode is required");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowError("Product name is required");
                return false;
            }

            if (numSalePrice.Value <= 0)
            {
                ShowError("Sale price must be greater than 0");
                return false;
            }

            return true;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}