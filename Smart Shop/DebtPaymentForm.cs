using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Smart_Shop
{
    public partial class DebtPaymentForm : Form
    {
        public string CustomerName => txtName.Text;
        public string CustomerPhone => txtPhone.Text;

        public DebtPaymentForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter customer name", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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