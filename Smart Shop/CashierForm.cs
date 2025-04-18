using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Data;

namespace Smart_Shop
{
    public partial class CashierForm : Form
    {
        private string currentUsername;
        private List<CartItem> cartItems = new List<CartItem>();

        public CashierForm(string username)
        {
            InitializeComponent();
            currentUsername = username;
            InitializeSalesTab();
            InitializeRequestsTab();
            LoadProducts();
        }

        private void InitializeSalesTab()
        {
            tabSales.Controls.Clear();

            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 200
            };

            // Products Grid
            DataGridView productsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                Name = "productsGrid",
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            productsGrid.CellDoubleClick += AddToCart;

            // Cart Panel
            Panel cartPanel = new Panel { Dock = DockStyle.Fill };

            // Cart Grid
            DataGridView cartGrid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 150,
                Name = "cartGrid",
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Payment Controls
            GroupBox paymentGroup = new GroupBox
            {
                Text = "Payment",
                Dock = DockStyle.Top,
                Height = 100
            };

            ComboBox paymentMethod = new ComboBox
            {
                Dock = DockStyle.Top,
                Items = { "Cash", "Debt" },
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Button completeSale = new Button
            {
                Text = "Complete Sale",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            completeSale.Click += CompleteSale_Click;

            // Layout
            paymentGroup.Controls.Add(paymentMethod);
            cartPanel.Controls.Add(completeSale);
            cartPanel.Controls.Add(paymentGroup);
            cartPanel.Controls.Add(cartGrid);

            splitContainer.Panel1.Controls.Add(productsGrid);
            splitContainer.Panel2.Controls.Add(cartPanel);
            tabSales.Controls.Add(splitContainer);
        }

        private void InitializeRequestsTab()
        {
            tabRequests.Controls.Clear();

            FlowLayoutPanel requestPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown
            };

            // Request Type Selection
            ComboBox requestType = new ComboBox
            {
                Items = { "Restock Request", "Debt Payment Request" },
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200
            };

            // Content Input
            TextBox requestContent = new TextBox
            {
                Multiline = true,
                Height = 100,
                Width = 300
            };

            // Submit Button
            Button submitRequest = new Button
            {
                Text = "Submit Request",
                Width = 150
            };
            submitRequest.Click += (s, e) => SubmitRequest(requestType.Text, requestContent.Text);

            requestPanel.Controls.Add(new Label { Text = "Request Type:" });
            requestPanel.Controls.Add(requestType);
            requestPanel.Controls.Add(new Label { Text = "Details:" });
            requestPanel.Controls.Add(requestContent);
            requestPanel.Controls.Add(submitRequest);

            tabRequests.Controls.Add(requestPanel);
        }

        private void LoadProducts()
        {
            if (tabSales.Controls[0] is SplitContainer splitContainer)
            {
                if (splitContainer.Panel1.Controls[0] is DataGridView productsGrid)
                {
                    productsGrid.DataSource = SQLiteDatabase.GetProducts();
                }
            }
        }

        private void AddToCart(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridView grid = (DataGridView)sender;
                DataRowView row = (DataRowView)grid.Rows[e.RowIndex].DataBoundItem;

                CartItem item = new CartItem
                {
                    ProductId = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString(),
                    Price = Convert.ToDecimal(row["Price"]),
                    Quantity = 1
                };

                cartItems.Add(item);
                RefreshCart();
            }
        }

        private void RefreshCart()
        {
            if (tabSales.Controls[0] is SplitContainer splitContainer)
            {
                if (splitContainer.Panel2.Controls[0] is Panel cartPanel)
                {
                    foreach (Control control in cartPanel.Controls)
                    {
                        if (control is DataGridView cartGrid)
                        {
                            cartGrid.DataSource = null;
                            cartGrid.DataSource = cartItems;
                        }
                    }
                }
            }
        }

        private void CompleteSale_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Please add items to cart", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get payment method
            string paymentMethod = "";
            foreach (Control control in ((Button)sender).Parent.Controls)
            {
                if (control is ComboBox comboBox)
                {
                    paymentMethod = comboBox.Text;
                    break;
                }
            }

            if (paymentMethod == "Debt")
            {
                DebtPaymentForm debtForm = new DebtPaymentForm();
                if (debtForm.ShowDialog() == DialogResult.OK)
                {
                    ProcessSale(paymentMethod, debtForm.CustomerName, debtForm.CustomerPhone);
                }
            }
            else
            {
                ProcessSale(paymentMethod);
            }
        }

        private void ProcessSale(string paymentMethod, string customerName = "", string customerPhone = "")
        {
            try
            {
                // Process each item in cart
                foreach (CartItem item in cartItems)
                {
                    SQLiteDatabase.RecordSale(item.ProductId, item.Quantity, item.Price,
                        paymentMethod, currentUsername);
                }

                // If debt, record it
                if (paymentMethod == "Debt")
                {
                    decimal total = cartItems.Sum(i => i.Total);
                    SQLiteDatabase.RecordDebt(customerName, customerPhone, total, currentUsername);
                }

                MessageBox.Show("Sale completed successfully", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                cartItems.Clear();
                RefreshCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SubmitRequest(string type, string content)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Please select request type and provide details", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SQLiteDatabase.SubmitRequest(type, content, currentUsername);
            MessageBox.Show("Request submitted successfully", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    public class CartItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total => Price * Quantity;
    }
}