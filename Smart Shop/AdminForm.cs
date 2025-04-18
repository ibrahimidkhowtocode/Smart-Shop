using System;
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
            db = new SQLiteDatabase();
            InitializeUI();
        }

        private void InitializeUI()
        {
            mainTabControl = new TabControl { Dock = DockStyle.Fill };
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
            // Implement history tab
        }

        private void InitializeExpensesTab(TabPage tabPage)
        {
            // Implement expenses tab
        }

        private void InitializeCashiersTab(TabPage tabPage)
        {
            // Implement cashiers tab
        }
    }
}