using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;

namespace Smart_Shop
{
    public partial class ProductsForm : Form
    {
        public ProductsForm()
        {
            InitializeComponent();
            ConfigureGrid();
            LoadProducts();
        }

        private void ConfigureGrid()
        {
            dgvProducts.AutoGenerateColumns = false;
            dgvProducts.Columns.Clear();

            // Explicitly define all columns
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                DataPropertyName = "Id",
                HeaderText = "ID",
                Width = 50,
                Visible = false // Hide ID column if preferred
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "Product Name",
                Width = 200,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Barcode",
                DataPropertyName = "Barcode",
                HeaderText = "Barcode",
                Width = 120
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PurchasePrice",
                DataPropertyName = "PurchasePrice",
                HeaderText = "Cost",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SalePrice",
                DataPropertyName = "SalePrice",
                HeaderText = "Price",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StockQuantity",
                DataPropertyName = "StockQuantity",
                HeaderText = "Stock"
            });

            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MinimumStockLevel",
                DataPropertyName = "MinimumStockLevel",
                HeaderText = "Min Stock"
            });

            dgvProducts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProducts.ReadOnly = true;
        }

        private void LoadProducts()
        {
            try
            {
                using var connection = new SQLiteConnection("Data Source=StoreDB.db");
                var adapter = new SQLiteDataAdapter("SELECT * FROM Products", connection);
                var dt = new DataTable();
                adapter.Fill(dt);

                // Verify critical columns exist
                if (!dt.Columns.Contains("Id") || !dt.Columns.Contains("Name"))
                {
                    MessageBox.Show("Database table is missing required columns", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dgvProducts.DataSource = dt;
                HighlightLowStock();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HighlightLowStock()
        {
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["StockQuantity"].Value != null &&
                    row.Cells["MinimumStockLevel"].Value != null)
                {
                    int stock = Convert.ToInt32(row.Cells["StockQuantity"].Value);
                    int minStock = Convert.ToInt32(row.Cells["MinimumStockLevel"].Value);

                    if (stock <= minStock)
                    {
                        row.DefaultCellStyle.BackColor = Color.LightCoral;
                        row.DefaultCellStyle.Font = new Font(dgvProducts.Font, FontStyle.Bold);
                    }
                }
            }
        }

        private void DeleteProduct()
        {
            if (dgvProducts.SelectedRows.Count == 0) return;

            try
            {
                var selectedRow = dgvProducts.SelectedRows[0];

                // Multiple ways to get ID for robustness
                int id = -1;
                if (selectedRow.Cells["Id"].Value != null)
                {
                    id = Convert.ToInt32(selectedRow.Cells["Id"].Value);
                }
                else if (selectedRow.DataBoundItem is DataRowView rowView)
                {
                    id = Convert.ToInt32(rowView["Id"]);
                }

                if (id == -1)
                {
                    MessageBox.Show("Could not determine product ID", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("Delete selected product?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    using var connection = new SQLiteConnection("Data Source=StoreDB.db");
                    connection.Open();
                    using var command = new SQLiteCommand(
                        "DELETE FROM Products WHERE Id = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Product deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ... [Keep all other existing methods unchanged] ...
        // BtnAdd_Click, BtnEdit_Click, BtnRefresh_Click, etc.
        // ExportToExcel, ImportFromExcel, etc.
    }
}