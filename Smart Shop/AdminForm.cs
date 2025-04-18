using System;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class AdminForm : Form
    {
        private readonly SQLiteDatabase db;
        private readonly TabControl mainTabControl = new TabControl();

        public AdminForm()
        {
            InitializeComponent();
            db = new SQLiteDatabase();
            InitializeUI();
        }

        private void InitializeUI()
        {
            mainTabControl.Dock = DockStyle.Fill;
            Controls.Add(mainTabControl);

            AddTab("Products", InitializeProductsTab);
            AddTab("History", InitializeHistoryTab);
            AddTab("Expenses", InitializeExpensesTab);
            AddTab("Cashiers", InitializeCashiersTab);
        }

        private void AddTab(string text, Action<TabPage> initializeAction)
        {
            var tabPage = new TabPage(text);
            mainTabControl.TabPages.Add(tabPage);
            initializeAction(tabPage);
        }

        private void InitializeProductsTab(TabPage tabPage)
        {
            var productsForm = new ProductsForm(db) { TopLevel = false };
            tabPage.Controls.Add(productsForm);
            productsForm.Show();
        }

        private void InitializeHistoryTab(TabPage tabPage)
        {
            // Your existing history tab implementation
        }

        private void InitializeExpensesTab(TabPage tabPage)
        {
            // Your existing expenses tab implementation
        }

        private void InitializeCashiersTab(TabPage tabPage)
        {
            // Your existing cashiers tab implementation
        }
    }
}