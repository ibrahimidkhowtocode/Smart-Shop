using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.ComponentModel;

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
            InitializeTabs();
            LoadProducts();
        }

        private void InitializeTabs()
        {
            tabControl1.TabPages.Clear();

            TabPage tabSales = new TabPage("Sales");
            tabSales.Controls.Add(CreateSalesTab());
            tabControl1.TabPages.Add(tabSales);

            TabPage tabRequests = new TabPage("Requests");
            tabRequests.Controls.Add(CreateRequestsTab());
            tabControl1.TabPages.Add(tabRequests);

            TabPage tabDebts = new TabPage("Debts");
            tabDebts.Controls.Add(CreateDebtsTab());
            tabControl1.TabPages.Add(tabDebts);
        }

        private Control CreateSalesTab()
        {
            SplitContainer splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterDistance = 200,
                MinimumSize = new Size(800, 600)
            };

            DataGridView productsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                Name = "productsGrid",
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                DataSource = SQLiteDatabase.GetProducts(),
                MinimumSize = new Size(600, 300)
            };
            productsGrid.CellDoubleClick += AddToCart;

            Panel manualAddPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            TextBox txtBarcode = new TextBox
            {
                Dock = DockStyle.Left,
                Width = 150,
                PlaceholderText = "Enter barcode"
            };

            Button btnScan = new Button
            {
                Text = "Scan",
                Dock = DockStyle.Left,
                Width = 80
            };
            btnScan.Click += (s, e) => ScanBarcode(txtBarcode);

            Button btnManualAdd = new Button
            {
                Text = "Add Item",
                Dock = DockStyle.Left,
                Width = 80
            };
            btnManualAdd.Click += (s, e) => ShowManualAddDialog();

            manualAddPanel.Controls.Add(btnManualAdd);
            manualAddPanel.Controls.Add(btnScan);
            manualAddPanel.Controls.Add(txtBarcode);

            Panel cartPanel = new Panel { Dock = DockStyle.Fill };

            DataGridView cartGrid = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 150,
                Name = "cartGrid",
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                MinimumSize = new Size(600, 150)
            };

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

            paymentGroup.Controls.Add(paymentMethod);
            cartPanel.Controls.Add(completeSale);
            cartPanel.Controls.Add(paymentGroup);
            cartPanel.Controls.Add(cartGrid);

            splitContainer.Panel1.Controls.Add(manualAddPanel);
            splitContainer.Panel1.Controls.Add(productsGrid);
            splitContainer.Panel2.Controls.Add(cartPanel);

            return splitContainer;
        }

        private Control CreateRequestsTab()
        {
            FlowLayoutPanel requestPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            ComboBox requestType = new ComboBox
            {
                Items = { "Restock Request", "Debt Payment Request" },
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 200
            };

            TextBox requestContent = new TextBox
            {
                Multiline = true,
                Height = 100,
                Width = 300
            };

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

            return requestPanel;
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
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            FlowLayoutPanel paymentPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(10)
            };

            NumericUpDown numPayment = new NumericUpDown
            {
                DecimalPlaces = 2,
                Width = 150,
                Maximum = 1000000
            };

            Button btnRecordPayment = new Button
            {
                Text = "Record Payment",
                Width = 150
            };
            btnRecordPayment.Click += (s, e) =>
            {
                if (debtsGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a debt first", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                RecordDebtPayment(debtsGrid, numPayment.Value);
            };

            paymentPanel.Controls.Add(new Label { Text = "Payment Amount:" });
            paymentPanel.Controls.Add(numPayment);
            paymentPanel.Controls.Add(btnRecordPayment);

            splitContainer.Panel1.Controls.Add(debtsGrid);
            splitContainer.Panel2.Controls.Add(paymentPanel);

            return splitContainer;
        }

        private void LoadProducts()
        {
            if (tabControl1.TabPages["Sales"]?.Controls[0] is SplitContainer salesContainer &&
                salesContainer.Panel1.Controls["productsGrid"] is DataGridView productsGrid)
            {
                productsGrid.DataSource = SQLiteDatabase.GetProducts();
            }
        }

        private void AddToCart(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && sender is DataGridView grid &&
                grid.Rows[e.RowIndex].DataBoundItem is DataRowView row)
            {
                AddProductToCart(
                    Convert.ToInt32(row["Id"]),
                    row["Name"]?.ToString() ?? string.Empty,
                    Convert.ToDecimal(row["Price"])
                );
            }
        }

        private void AddProductToCart(int productId, string name, decimal price)
        {
            var existingItem = cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Name = name,
                    Price = price,
                    Quantity = 1
                });
            }
            RefreshCart();
        }

        private void RefreshCart()
        {
            if (tabControl1.TabPages["Sales"]?.Controls[0] is SplitContainer salesContainer)
            {
                var cartPanel = salesContainer.Panel2.Controls.OfType<Panel>().FirstOrDefault();
                if (cartPanel != null)
                {
                    var cartGrid = cartPanel.Controls.OfType<DataGridView>().FirstOrDefault();
                    if (cartGrid != null)
                    {
                        cartGrid.DataSource = null;
                        cartGrid.DataSource = new BindingSource { DataSource = cartItems };
                        cartGrid.Columns["Total"]!.HeaderText = "Total Price";
                        cartGrid.Refresh();
                    }
                }
            }
        }

        private void CompleteSale_Click(object? sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                MessageBox.Show("Please add items to cart", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string paymentMethod = "";
            if (sender is Button button && button.Parent is Panel panel)
            {
                foreach (Control control in panel.Controls)
                {
                    if (control is GroupBox group && group.Controls.Count > 0 && group.Controls[0] is ComboBox combo)
                    {
                        paymentMethod = combo.Text;
                        break;
                    }
                }
            }

            try
            {
                if (paymentMethod == "Debt")
                {
                    using (var debtForm = new DebtPaymentForm())
                    {
                        if (debtForm.ShowDialog() == DialogResult.OK)
                        {
                            ProcessSale(paymentMethod, debtForm.CustomerName, debtForm.CustomerPhone);
                        }
                    }
                }
                else
                {
                    ProcessSale(paymentMethod);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessSale(string paymentMethod, string customerName = "", string customerPhone = "")
        {
            try
            {
                foreach (CartItem item in cartItems)
                {
                    SQLiteDatabase.RecordSale(item.ProductId, item.Quantity, item.Price,
                        paymentMethod, currentUsername);
                }

                if (paymentMethod == "Debt")
                {
                    decimal total = cartItems.Sum(i => i.Price * i.Quantity);
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

        private void ScanBarcode(TextBox txtBarcode)
        {
            string barcode = txtBarcode.Text;
            if (!string.IsNullOrEmpty(barcode))
            {
                try
                {
                    var product = SQLiteDatabase.GetProductByBarcode(barcode);
                    if (product != null)
                    {
                        AddProductToCart(product.Id, product.Name, product.Price);
                        txtBarcode.Clear();
                        MessageBox.Show("Product scanned successfully!", "Scan Result",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No product found with this barcode", "Scan Result",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Scanning error: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ShowManualAddDialog()
        {
            using (var form = new Form())
            {
                form.Text = "Add Item Manually";
                form.Size = new Size(400, 250);
                form.StartPosition = FormStartPosition.CenterParent;

                Panel searchPanel = new Panel { Dock = DockStyle.Top, Height = 40 };
                TextBox txtSearch = new TextBox
                {
                    Dock = DockStyle.Fill,
                    PlaceholderText = "Search products..."
                };
                searchPanel.Controls.Add(txtSearch);

                ComboBox cmbProducts = new ComboBox
                {
                    Dock = DockStyle.Top,
                    DisplayMember = "Name",
                    ValueMember = "Id",
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                NumericUpDown numQuantity = new NumericUpDown
                {
                    Minimum = 1,
                    Value = 1,
                    Dock = DockStyle.Top
                };

                Button btnAdd = new Button { Text = "Add", Dock = DockStyle.Bottom };

                var products = SQLiteDatabase.GetProducts();
                if (products != null)
                {
                    cmbProducts.DataSource = products;

                    txtSearch.TextChanged += (s, e) =>
                    {
                        if (string.IsNullOrWhiteSpace(txtSearch.Text))
                        {
                            cmbProducts.DataSource = products;
                        }
                        else
                        {
                            var filteredRows = products.AsEnumerable()
                                .Where(row => row.Field<string>("Name")?.IndexOf(txtSearch.Text, StringComparison.OrdinalIgnoreCase) >= 0);
                            cmbProducts.DataSource = filteredRows.Any() ? filteredRows.CopyToDataTable() : new DataTable();
                        }
                    };
                }

                btnAdd.Click += (s, e) =>
                {
                    if (cmbProducts.SelectedItem is DataRowView row &&
                        row["Name"] != null &&
                        row["Price"] != null)
                    {
                        AddProductToCart(
                            Convert.ToInt32(row["Id"]),
                            row["Name"].ToString() ?? string.Empty,
                            Convert.ToDecimal(row["Price"])
                        );
                        form.DialogResult = DialogResult.OK;
                    }
                };

                form.Controls.Add(numQuantity);
                form.Controls.Add(cmbProducts);
                form.Controls.Add(searchPanel);
                form.Controls.Add(btnAdd);

                form.ShowDialog();
            }
        }

        private void RecordDebtPayment(DataGridView debtsGrid, decimal amount)
        {
            if (debtsGrid.SelectedRows.Count > 0 &&
                debtsGrid.SelectedRows[0].DataBoundItem is DataRowView row)
            {
                int debtId = Convert.ToInt32(row["Id"]);
                SQLiteDatabase.RecordDebtPayment(debtId, amount, currentUsername);
                debtsGrid.DataSource = SQLiteDatabase.GetDebts();
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

        public class CartItem
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Total => Price * Quantity;
        }
    }
}