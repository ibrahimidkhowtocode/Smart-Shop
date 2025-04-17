using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class AdminForm : Form
    {
        private readonly SQLiteDatabase db;
        private TabControl tabControl;

        public AdminForm()
        {
            InitializeComponent();
            db = new SQLiteDatabase("SmartShop.db");
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            this.Text = "Admin Panel";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl { Dock = DockStyle.Fill };
            this.Controls.Add(tabControl);

            AddProductsTab();
            AddDebtsTab();
            AddHistoryTab();
            AddExpensesTab();
            AddCashiersTab();
        }

        private void AddProductsTab()
        {
            var tab = new TabPage("Products");
            tabControl.TabPages.Add(tab);

            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            var btnAdd = new Button { Text = "Add Product", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => AddProduct();
            tab.Controls.Add(btnAdd);

            dgv.DataSource = db.GetDataTable("SELECT * FROM products");
        }

        private void AddProduct()
        {
            using (var form = new Form())
            {
                form.Text = "Add New Product";
                form.Size = new Size(300, 200);

                var txtName = new TextBox { PlaceholderText = "Name", Dock = DockStyle.Top };
                var txtPrice = new TextBox { PlaceholderText = "Price", Dock = DockStyle.Top };
                var btnSave = new Button { Text = "Save", Dock = DockStyle.Bottom };

                btnSave.Click += (s, e) =>
                {
                    db.ExecuteNonQuery(
                        "INSERT INTO products (name, price) VALUES (@name, @price)",
                        new SQLiteParameter("@name", txtName.Text),
                        new SQLiteParameter("@price", double.Parse(txtPrice.Text))
                    );
                    form.DialogResult = DialogResult.OK;
                };

                form.Controls.AddRange(new Control[] { txtName, txtPrice, btnSave });
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }

        private void LoadData()
        {
            // Refresh all tabs
            foreach (TabPage tab in tabControl.TabPages)
            {
                if (tab.Controls.Count > 0 && tab.Controls[0] is DataGridView dgv)
                {
                    dgv.DataSource = db.GetDataTable($"SELECT * FROM {tab.Text.ToLower()}");
                }
            }
        }

        private void AddDebtsTab()
        {
            var tab = new TabPage("Debts");
            tabControl.TabPages.Add(tab);

            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            dgv.DataSource = db.GetDataTable("SELECT * FROM debts");
        }

        private void AddHistoryTab()
        {
            var tab = new TabPage("History");
            tabControl.TabPages.Add(tab);

            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            dgv.DataSource = db.GetDataTable("SELECT * FROM history ORDER BY timestamp DESC");
        }

        private void AddExpensesTab()
        {
            var tab = new TabPage("Expenses");
            tabControl.TabPages.Add(tab);

            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            var btnAdd = new Button { Text = "Add Expense", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => AddExpense();
            tab.Controls.Add(btnAdd);

            dgv.DataSource = db.GetDataTable("SELECT * FROM expenses ORDER BY date DESC");
        }

        private void AddExpense()
        {
            using (var form = new Form())
            {
                form.Text = "Add New Expense";
                form.Size = new Size(300, 200);

                var txtDesc = new TextBox { PlaceholderText = "Description", Dock = DockStyle.Top };
                var txtAmount = new TextBox { PlaceholderText = "Amount", Dock = DockStyle.Top };
                var btnSave = new Button { Text = "Save", Dock = DockStyle.Bottom };

                btnSave.Click += (s, e) =>
                {
                    db.AddExpense(txtDesc.Text, double.Parse(txtAmount.Text));
                    form.DialogResult = DialogResult.OK;
                };

                form.Controls.AddRange(new Control[] { txtDesc, txtAmount, btnSave });
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }

        private void AddCashiersTab()
        {
            var tab = new TabPage("Cashiers");
            tabControl.TabPages.Add(tab);

            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            var btnAdd = new Button { Text = "Add Cashier", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => AddCashier();
            tab.Controls.Add(btnAdd);

            dgv.DataSource = db.GetDataTable("SELECT * FROM cashiers");
        }

        private void AddCashier()
        {
            using (var form = new Form())
            {
                form.Text = "Add New Cashier";
                form.Size = new Size(300, 250);

                var txtUser = new TextBox { PlaceholderText = "Username", Dock = DockStyle.Top };
                var txtPass = new TextBox { PlaceholderText = "Password", Dock = DockStyle.Top };
                var cmbWageType = new ComboBox { Dock = DockStyle.Top };
                cmbWageType.Items.AddRange(new[] { "monthly", "hourly" });
                var txtWageRate = new TextBox { PlaceholderText = "Wage Rate", Dock = DockStyle.Top };
                var btnSave = new Button { Text = "Save", Dock = DockStyle.Bottom };

                btnSave.Click += (s, e) =>
                {
                    db.ExecuteNonQuery(
                        "INSERT INTO cashiers (username, password, wage_type, wage_rate) VALUES (@user, @pass, @type, @rate)",
                        new SQLiteParameter("@user", txtUser.Text),
                        new SQLiteParameter("@pass", txtPass.Text),
                        new SQLiteParameter("@type", cmbWageType.Text),
                        new SQLiteParameter("@rate", double.Parse(txtWageRate.Text))
                    );
                    form.DialogResult = DialogResult.OK;
                };

                form.Controls.AddRange(new Control[] { txtUser, txtPass, cmbWageType, txtWageRate, btnSave });
                if (form.ShowDialog() == DialogResult.OK)
                    LoadData();
            }
        }
    }
}