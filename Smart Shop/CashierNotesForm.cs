using System;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class CashierNotesForm : Form
    {
        public string Notes => txtNotes.Text;

        public CashierNotesForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                MessageBox.Show("Please enter your shift notes", "Error",
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