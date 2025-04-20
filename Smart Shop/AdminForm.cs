using System;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ComponentModel;

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
            tabControl1.TabPages.Clear();

            TabPage tabSummary = new TabPage("Summary");
            tabSummary.Controls.Add(CreateSummaryTab());
            tabControl1.TabPages.Add(tabSummary);

            TabPage tabBusiness = new TabPage("Business");
            tabBusiness.Controls.Add(CreateBusinessTab());
            tabControl1.TabPages.Add(tabBusiness);

            TabPage tabExpenses = new TabPage("Expenses");
            tabExpenses.Controls.Add(CreateExpensesTab());
            tabControl1.TabPages.Add(tabExpenses);

            TabPage tabRequests = new TabPage("Requests");
            tabRequests.Controls.Add(CreateRequestsTab());
            tabControl1.TabPages.Add(tabRequests);

            TabPage tabDebts = new TabPage("Debts");
            tabDebts.Controls.Add(CreateDebtsTab());
            tabControl1.TabPages.Add(tabDebts);

            TabPage tabNotes = new TabPage("Cashier Notes");
            tabNotes.Controls.Add(CreateNotesTab());
            tabControl1.TabPages.Add(tabNotes);
        }

        private Control CreateSummaryTab()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 300,
                MinimumSize = new Size(800, 600)
            };

            DataGridView summaryGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetSummaryData(),
                MinimumSize = new Size(600, 300)
            };

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
            TabControl businessTabs = new TabControl { Dock = DockStyle.Fill };

            TabPage productsTab = new TabPage("Products");
            productsTab.Controls.Add(CreateProductsPanel());
            businessTabs.TabPages.Add(productsTab);

            TabPage cashiersTab = new TabPage("Cashiers");
            cashiersTab.Controls.Add(CreateCashiersPanel());
            businessTabs.TabPages.Add(cashiersTab);

            return businessTabs;
        }

        private Control CreateProductsPanel()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400,
                MinimumSize = new Size(600, 400)
            };

            DataGridView productsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetProducts(),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Name = "productsGrid"
            };

            FlowLayoutPanel productControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            Button btnAddProduct = new Button { Text = "Add Product", Width = 150 };
            Button btnEditProduct = new Button { Text = "Edit Product", Width = 150 };
            Button btnDeleteProduct = new Button { Text = "Delete Product", Width = 150 };

            btnAddProduct.Click += (s, e) =>
            {
                var form = new AddEditProductForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    productsGrid.DataSource = SQLiteDatabase.GetProducts();
                    productsGrid.Refresh();
                    MessageBox.Show("Product added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnEditProduct.Click += (s, e) =>
            {
                if (productsGrid.SelectedRows.Count > 0)
                {
                    EditSelectedProduct(productsGrid);
                }
                else
                {
                    MessageBox.Show("Please select a product first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDeleteProduct.Click += (s, e) =>
            {
                if (productsGrid.SelectedRows.Count > 0)
                {
                    DeleteSelectedProduct(productsGrid);
                }
                else
                {
                    MessageBox.Show("Please select a product first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            productControls.Controls.AddRange(new Control[] { btnAddProduct, btnEditProduct, btnDeleteProduct });
            splitContainer.Panel1.Controls.Add(productsGrid);
            splitContainer.Panel2.Controls.Add(productControls);

            return splitContainer;
        }

        private Control CreateCashiersPanel()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400,
                MinimumSize = new Size(600, 400)
            };

            DataGridView cashiersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetCashiers(),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Name = "cashiersGrid"
            };

            FlowLayoutPanel managementPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            Button btnAddCashier = new Button { Text = "Add Cashier", Width = 150 };
            Button btnEditCashier = new Button { Text = "Edit Cashier", Width = 150 };
            Button btnDeactivate = new Button { Text = "Deactivate", Width = 150 };

            btnAddCashier.Click += (s, e) =>
            {
                var form = new CashierAccountForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    cashiersGrid.DataSource = SQLiteDatabase.GetCashiers();
                    cashiersGrid.Refresh();
                    MessageBox.Show("Cashier added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnEditCashier.Click += (s, e) =>
            {
                if (cashiersGrid.SelectedRows.Count > 0)
                {
                    EditSelectedCashier(cashiersGrid);
                }
                else
                {
                    MessageBox.Show("Please select a cashier first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDeactivate.Click += (s, e) =>
            {
                if (cashiersGrid.SelectedRows.Count > 0)
                {
                    DeactivateCashier(cashiersGrid);
                }
                else
                {
                    MessageBox.Show("Please select a cashier first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

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
                Orientation = Orientation.Vertical,
                SplitterDistance = 400,
                MinimumSize = new Size(600, 400)
            };

            DataGridView expensesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetExpenses(),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Name = "expensesGrid"
            };

            FlowLayoutPanel expenseControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            Button btnAddExpense = new Button { Text = "Add Expense", Width = 150 };
            Button btnEditExpense = new Button { Text = "Edit Expense", Width = 150 };
            Button btnDeleteExpense = new Button { Text = "Delete Expense", Width = 150 };

            btnAddExpense.Click += (s, e) =>
            {
                var form = new ExpenseForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    expensesGrid.DataSource = SQLiteDatabase.GetExpenses();
                    expensesGrid.Refresh();
                    MessageBox.Show("Expense added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnEditExpense.Click += (s, e) =>
            {
                if (expensesGrid.SelectedRows.Count > 0)
                {
                    EditSelectedExpense(expensesGrid);
                }
                else
                {
                    MessageBox.Show("Please select an expense first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDeleteExpense.Click += (s, e) =>
            {
                if (expensesGrid.SelectedRows.Count > 0)
                {
                    DeleteSelectedExpense(expensesGrid);
                }
                else
                {
                    MessageBox.Show("Please select an expense first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            expenseControls.Controls.AddRange(new Control[] { btnAddExpense, btnEditExpense, btnDeleteExpense });
            splitContainer.Panel1.Controls.Add(expensesGrid);
            splitContainer.Panel2.Controls.Add(expenseControls);

            return splitContainer;
        }

        private Control CreateRequestsTab()
        {
            DataGridView requestsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetDataTable(@"SELECT 
                    r.Id,
                    r.Type AS [Type],
                    r.Content AS Request,
                    u.Username AS Cashier,
                    r.CreatedDate AS [Date],
                    r.Status
                    FROM Requests r
                    JOIN Users u ON r.CreatedBy = u.Id
                    ORDER BY r.CreatedDate DESC"),
                MinimumSize = new Size(600, 400),
                AllowUserToResizeRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
            };

            FlowLayoutPanel requestControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.LeftToRight
            };

            Button btnProcessRequest = new Button { Text = "Process Request", Width = 150 };
            btnProcessRequest.Click += (s, e) =>
            {
                if (requestsGrid.SelectedRows.Count > 0)
                {
                    ProcessSelectedRequest(requestsGrid);
                }
                else
                {
                    MessageBox.Show("Please select a request first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            requestControls.Controls.Add(btnProcessRequest);

            Panel panel = new Panel();
            panel.Controls.Add(requestsGrid);
            panel.Controls.Add(requestControls);

            return panel;
        }

        private Control CreateDebtsTab()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 400,
                MinimumSize = new Size(600, 400)
            };

            DataGridView debtsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetDebts(),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Name = "debtsGrid"
            };

            debtsGrid.DataBindingComplete += (s, e) => {
                debtsGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                if (debtsGrid.Columns.Count > 0)
                {
                    debtsGrid.Columns["Remaining"].DefaultCellStyle.ForeColor = Color.Red;
                    debtsGrid.Columns["Remaining"].DefaultCellStyle.Font =
                        new Font(debtsGrid.Font, FontStyle.Bold);
                }
            };

            FlowLayoutPanel debtControls = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            NumericUpDown numPayment = new NumericUpDown
            {
                Width = 150,
                DecimalPlaces = 2,
                Maximum = 1000000
            };

            Button btnRecordPayment = new Button { Text = "Record Payment", Width = 150 };
            Button btnAddDebt = new Button { Text = "Add New Debt", Width = 150 };

            btnRecordPayment.Click += (s, e) =>
            {
                if (debtsGrid.SelectedRows.Count > 0)
                {
                    RecordDebtPayment(debtsGrid, numPayment.Value);
                }
                else
                {
                    MessageBox.Show("Please select a debt first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnAddDebt.Click += (s, e) =>
            {
                var form = new DebtPaymentForm();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    debtsGrid.DataSource = SQLiteDatabase.GetDebts();
                    debtsGrid.Refresh();
                    MessageBox.Show("Debt added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            debtControls.Controls.Add(new Label { Text = "Payment Amount:" });
            debtControls.Controls.Add(numPayment);
            debtControls.Controls.Add(btnRecordPayment);
            debtControls.Controls.Add(btnAddDebt);

            splitContainer.Panel1.Controls.Add(debtsGrid);
            splitContainer.Panel2.Controls.Add(debtControls);

            return splitContainer;
        }

        private Control CreateNotesTab()
        {
            DataGridView notesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = SQLiteDatabase.GetCashierNotes(),
                MinimumSize = new Size(600, 400)
            };

            notesGrid.DataBindingComplete += (s, e) => {
                notesGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            };

            return notesGrid;
        }

        private void LoadData()
        {
            foreach (TabPage tab in tabControl1.TabPages)
            {
                RefreshDataGrids(tab);
            }
        }

        private void RefreshDataGrids(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control is DataGridView grid)
                {
                    if (grid.Name == "productsGrid") grid.DataSource = SQLiteDatabase.GetProducts();
                    else if (grid.Name == "cashiersGrid") grid.DataSource = SQLiteDatabase.GetCashiers();
                    else if (grid.Name == "expensesGrid") grid.DataSource = SQLiteDatabase.GetExpenses();
                    else if (grid.Name == "debtsGrid") grid.DataSource = SQLiteDatabase.GetDebts();
                    else if (grid.Name.Contains("note")) grid.DataSource = SQLiteDatabase.GetCashierNotes();
                    else if (grid.Name.Contains("request"))
                        grid.DataSource = SQLiteDatabase.GetDataTable("SELECT * FROM Requests ORDER BY Status, CreatedDate DESC");
                    grid.Refresh();
                }
                else if (control is SplitContainer split)
                {
                    RefreshDataGrids(split.Panel1);
                    RefreshDataGrids(split.Panel2);
                }
                else if (control is TabControl tabs)
                {
                    foreach (TabPage tab in tabs.TabPages)
                    {
                        RefreshDataGrids(tab);
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

        private void EditSelectedProduct(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                var form = new AddEditProductForm(
                    id: Convert.ToInt32(row["Id"]),
                    name: row["Name"]?.ToString() ?? string.Empty,
                    price: Convert.ToDecimal(row["Price"]),
                    cost: Convert.ToDecimal(row["Cost"]),
                    quantity: Convert.ToInt32(row["Quantity"]),
                    barcode: row["Barcode"]?.ToString() ?? string.Empty
                );

                if (form.ShowDialog() == DialogResult.OK)
                {
                    grid.DataSource = SQLiteDatabase.GetProducts();
                    grid.Refresh();
                    MessageBox.Show("Product updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DeleteSelectedProduct(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                if (MessageBox.Show("Delete this product?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SQLiteDatabase.DeleteProduct(Convert.ToInt32(row["Id"]));
                    grid.DataSource = SQLiteDatabase.GetProducts();
                    grid.Refresh();
                    MessageBox.Show("Product deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void EditSelectedCashier(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row && row["Username"] is string username)
            {
                var form = new CashierAccountForm(username);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    grid.DataSource = SQLiteDatabase.GetCashiers();
                    grid.Refresh();
                    MessageBox.Show("Cashier updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DeactivateCashier(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row && row["Username"] is string username)
            {
                if (MessageBox.Show("Deactivate this cashier?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SQLiteDatabase.DeactivateUser(username);
                    grid.DataSource = SQLiteDatabase.GetCashiers();
                    grid.Refresh();
                    MessageBox.Show("Cashier deactivated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void EditSelectedExpense(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                var form = new ExpenseForm(Convert.ToInt32(row["Id"]));
                if (form.ShowDialog() == DialogResult.OK)
                {
                    grid.DataSource = SQLiteDatabase.GetExpenses();
                    grid.Refresh();
                    MessageBox.Show("Expense updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void DeleteSelectedExpense(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                if (MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Deletion",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SQLiteDatabase.DeleteExpense(Convert.ToInt32(row["Id"]));
                    grid.DataSource = SQLiteDatabase.GetExpenses();
                    grid.Refresh();
                    MessageBox.Show("Expense deleted successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ProcessSelectedRequest(DataGridView grid)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                int requestId = Convert.ToInt32(row["Id"]);
                SQLiteDatabase.ExecuteNonQuery(
                    "UPDATE Requests SET Status='Processed', ProcessedDate=CURRENT_TIMESTAMP WHERE Id=@id",
                    ("@id", requestId)
                );
                grid.DataSource = SQLiteDatabase.GetDataTable("SELECT * FROM Requests ORDER BY Status, CreatedDate DESC");
                grid.Refresh();
                MessageBox.Show("Request processed successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RecordDebtPayment(DataGridView grid, decimal amount)
        {
            if (grid.SelectedRows.Count > 0 && grid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                int debtId = Convert.ToInt32(row["Id"]);
                SQLiteDatabase.RecordDebtPayment(debtId, amount, "admin");
                grid.DataSource = SQLiteDatabase.GetDebts();
                grid.Refresh();
                MessageBox.Show("Payment recorded successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}