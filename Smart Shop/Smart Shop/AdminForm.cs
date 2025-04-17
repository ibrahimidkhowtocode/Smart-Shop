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
        private TabControl mainTabControl;

        public AdminForm()
        {
            InitializeComponent();
            db = new SQLiteDatabase("SmartShop.db");
            InitializeUI();
            LoadData();
        }

        private void InitializeUI()
        {
            Text = "Admin Panel";
            Size = new Size(1000, 700);
            StartPosition = FormStartPosition.CenterScreen;

            mainTabControl = new TabControl { Dock = DockStyle.Fill };
            Controls.Add(mainTabControl);

            AddTab("Products", InitializeProductsTab);
            AddTab("Debts", InitializeDebtsTab);
            AddTab("History", InitializeHistoryTab);
            AddTab("Expenses", InitializeExpensesTab);
            AddTab("Cashiers", InitializeCashiersTab);
        }

        private void AddTab(string name, Action<TabPage> initializer)
        {
            var tab = new TabPage(name);
            mainTabControl.TabPages.Add(tab);
            initializer(tab);
        }

        private void InitializeProductsTab(TabPage tab)
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            var btnAdd = new Button { Text = "Add Product", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => ShowAddProductForm();
            tab.Controls.Add(btnAdd);

            dgv.DataSource = db.GetDataTable("SELECT * FROM products");
        }

        private void ShowAddProductForm()
        {
            using var form = new Form
            {
                Text = "Add New Product",
                Size = new Size(300, 200)
            };

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
            {
                LoadData();
            }
        }

        private void InitializeDebtsTab(TabPage tab)
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);
            dgv.DataSource = db.GetDataTable("SELECT * FROM debts");
        }

        private void InitializeHistoryTab(TabPage tab)
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);
            dgv.DataSource = db.GetDataTable("SELECT * FROM history ORDER BY timestamp DESC");
        }

        private void InitializeExpensesTab(TabPage tab)
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            var btnAdd = new Button { Text = "Add Expense", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => ShowAddExpenseForm();
            tab.Controls.Add(btnAdd);

            dgv.DataSource = db.GetDataTable("SELECT * FROM expenses ORDER BY date DESC");
        }

        private void ShowAddExpenseForm()
        {
            using var form = new Form
            {
                Text = "Add New Expense",
                Size = new Size(300, 200)
            };

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
            {
                LoadData();
            }
        }

        private void InitializeCashiersTab(TabPage tab)
        {
            var dgv = new DataGridView { Dock = DockStyle.Fill };
            tab.Controls.Add(dgv);

            var btnAdd = new Button { Text = "Add Cashier", Dock = DockStyle.Top };
            btnAdd.Click += (s, e) => ShowAddCashierForm();
            tab.Controls.Add(btnAdd);

            dgv.DataSource = db.GetDataTable("SELECT * FROM cashiers");
        }

        private void ShowAddCashierForm()
        {
            using var form = new Form
            {
                Text = "Add New Cashier",
                Size = new Size(300, 250)
            };

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
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            foreach (TabPage tab in mainTabControl.TabPages