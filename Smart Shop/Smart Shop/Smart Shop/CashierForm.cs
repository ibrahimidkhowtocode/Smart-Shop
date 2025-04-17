using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace Smart_Shop
{
    public partial class CashierForm : Form
    {
        private readonly SQLiteDatabase db;
        private readonly string cashierUsername;
        private readonly List<CartItem> cart = new();

        public CashierForm(string username)
        {
            InitializeComponent();
            db = new SQLiteDatabase("SmartShop.db");
            cashierUsername = username;
            InitializeUI();
            ShowNotesPopup();
        }

        private void ShowNotesPopup()
        {
            using (var form = new Form())
            {
                form.Text = "Shift Notes";
                form.Size = new Size(400, 300);

                var txtNotes = new TextBox { Multiline = true, Dock = DockStyle.Fill };
                var btnSave = new Button { Text = "Submit Notes", Dock = DockStyle.Bottom };

                btnSave.Click += (s, e) =>
                {
                    db.LogHistory("Cashier Note", txtNotes.Text, cashierUsername);
                    form.Close();
                };

                form.Controls.Add(txtNotes);
                form.Controls.Add(btnSave);
                form.ShowDialog();
            }
        }

        private void InitializeUI()
        {
            this.Text = $"Cashier: {cashierUsername}";
            this.Size = new Size(800, 600);

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical
            };
            this.Controls.Add(splitContainer);

            // Products Panel
            var dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = db.GetDataTable("SELECT id, name, price, stock FROM products")
            };
            splitContainer.Panel1.Controls.Add(dgvProducts);

            // Cart Panel
            var pnlCart = new Panel { Dock = DockStyle.Fill };
            splitContainer.Panel2.Controls.Add(pnlCart);

            var lstCart = new ListBox { Dock = DockStyle.Fill };
            pnlCart.Controls.Add(lstCart);

            var btnAddToCart = new Button
            {
                Text = "Add to Cart",
                Dock = DockStyle.Bottom
            };
            btnAddToCart.Click += BtnAddToCart_Click;
            pnlCart.Controls.Add(btnAddToCart);

            var btnCheckout = new Button
            {
                Text = "Checkout",
                Dock = DockStyle.Bottom
            };
            btnCheckout.Click += BtnCheckout_Click;
            pnlCart.Controls.Add(btnCheckout);
        }

        private void BtnAddToCart_Click(object sender, EventArgs e)
        {
            // Implementation for adding to cart
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            // Implementation for checkout
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get;