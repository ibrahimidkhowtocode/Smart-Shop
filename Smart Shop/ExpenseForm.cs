using System;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class ExpenseForm : Form
    {
        private int? expenseId;

        public ExpenseForm(int? id = null)
        {
            InitializeComponent();
            expenseId = id;
            this.StartPosition = FormStartPosition.CenterScreen;

            if (expenseId.HasValue)
            {
                this.Text = "Edit Expense";
                LoadExpenseData();
            }
            else
            {
                this.Text = "Add New Expense";
            }
        }

        private void LoadExpenseData()
        {
            // Load existing expense data
            var expense = SQLiteDatabase.GetExpense(expenseId.Value);
            txtDescription.Text = expense.Description;
            numAmount.Value = expense.Amount;
            txtCategory.Text = expense.Category;
            txtNotes.Text = expense.Notes;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Please enter description", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (numAmount.Value <= 0)
            {
                MessageBox.Show("Please enter valid amount", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (expenseId.HasValue)
            {
                SQLiteDatabase.UpdateExpense(expenseId.Value, txtDescription.Text,
                    numAmount.Value, txtCategory.Text, txtNotes.Text);
            }
            else
            {
                SQLiteDatabase.AddExpense(txtDescription.Text, numAmount.Value,
                    txtCategory.Text, txtNotes.Text);
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