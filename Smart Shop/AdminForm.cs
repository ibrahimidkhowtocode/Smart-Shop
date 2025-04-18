using System;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Linq;

namespace Smart_Shop
{
    public partial class AdminForm : Form
    {

        public AdminForm()
        {
            InitializeComponent();
            InitializeTabs();
            LoadData();
            CheckPendingRequests();
        }

        private void InitializeTabs()
        {
            // Summary Tab
            tabSummary.Controls.Add(CreateSummaryTab());

            // Business Management Tab
            tabBusiness.Controls.Add(CreateBusinessTab());

            // Expenses Tab
            tabExpenses.Controls.Add(CreateExpensesTab());

            // Cashier Notes Tab
            tabNotes.Controls.Add(CreateNotesTab());
        }

        private Control CreateSummaryTab()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };

            // Summary Data Grid
            DataGridView summaryGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetSummaryData()
            };

            // Chart Panel (placeholder)
            Panel chartPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            splitContainer.Panel1.Controls.Add(summaryGrid);
            splitContainer.Panel2.Controls.Add(chartPanel);

            return splitContainer;
        }

        private Control CreateBusinessTab()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };

            // Cashiers Data Grid
            DataGridView cashiersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetCashiers()
            };

            // Management Controls
            FlowLayoutPanel managementPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };

            Button btnAddCashier = new Button { Text = "Add Cashier", Width = 150 };
            Button btnEditCashier = new Button { Text = "Edit Cashier", Width = 150 };
            Button btnDeactivate = new Button { Text = "Deactivate", Width = 150 };

            btnAddCashier.Click += (s, e) => new CashierAccountForm().ShowDialog();
            btnEditCashier.Click += (s, e) => EditSelectedCashier(cashiersGrid);
            btnDeactivate.Click += (s, e) => DeactivateCashier(cashiersGrid);

            managementPanel.Controls.AddRange(new Control[] { btnAddCashier, btnEditCashier, btnDeactivate });

            splitContainer.Panel1.Controls.Add(cashiersGrid);
            splitContainer.Panel2.Controls.Add(managementPanel);

            return splitContainer;
        }

        private Control CreateExpensesTab()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };

            // Expenses Data Grid
            DataGridView expensesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetExpenses()
            };

            // Expense Controls
            FlowLayoutPanel expenseControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };

            Button btnAddExpense = new Button { Text = "Add Expense", Width = 150 };
            Button btnEditExpense = new Button { Text = "Edit Expense", Width = 150 };
            Button btnDeleteExpense = new Button { Text = "Delete Expense", Width = 150 };

            btnAddExpense.Click += (s, e) => new ExpenseForm().ShowDialog();
            btnEditExpense.Click += (s, e) => EditSelectedExpense(expensesGrid);
            btnDeleteExpense.Click += (s, e) => DeleteSelectedExpense(expensesGrid);

            expenseControls.Controls.AddRange(new Control[] { btnAddExpense, btnEditExpense, btnDeleteExpense });

            splitContainer.Panel1.Controls.Add(expensesGrid);
            splitContainer.Panel2.Controls.Add(expenseControls);

            return splitContainer;
        }

        private Control CreateNotesTab()
        {
            DataGridView notesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetCashierNotes()
            };

            return notesGrid;
        }

        private void LoadData()
        {
            foreach (TabPage tab in tabControl1.TabPages)
            {
                foreach (Control control in tab.Controls)
                {
                    if (control is DataGridView grid)
                    {
                        if (grid.Name.Contains("summary")) grid.DataSource = SQLiteDatabase.GetSummaryData();
                        else if (grid.Name.Contains("cashier")) grid.DataSource = SQLiteDatabase.GetCashiers();
                        else if (grid.Name.Contains("expense")) grid.DataSource = SQLiteDatabase.GetExpenses();
                        else if (grid.Name.Contains("note")) grid.DataSource = SQLiteDatabase.GetCashierNotes();
                    }
                }
            }
        }

        private void CheckPendingRequests()
        {
            int pendingCount = SQLiteDatabase.GetPendingRequestsCount();
            if (pendingCount > 0)
            {
                MessageBox.Show($"You have {pendingCount} pending requests",
                    "Pending Requests", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm().Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void EditSelectedCashier(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 &&
                grid.SelectedRows[0].DataBoundItem is DataRowView row &&
                row["Username"] is string username)
            {
                new CashierAccountForm(username).ShowDialog();
                LoadData();
            }
        }

        private void DeactivateCashier(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 &&
                grid.SelectedRows[0].DataBoundItem is DataRowView row &&
                row["Username"] is string username)
            {
                SQLiteDatabase.DeactivateUser(username);
                LoadData();
            }
        }

        private void EditSelectedExpense(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 &&
                grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                new ExpenseForm(Convert.ToInt32(row["Id"])).ShowDialog();
                LoadData();
            }
        }

        private void DeleteSelectedExpense(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 &&
                grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                SQLiteDatabase.DeleteExpense(Convert.ToInt32(row["Id"]));
                LoadData();
            }
        }
    }
}