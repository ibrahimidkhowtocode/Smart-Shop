using System;
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;

namespace Smart_Shop
{
    public static class SQLiteDatabase
    {
        private static string connectionString = "Data Source=SmartShop.db;Version=3;";

        // Helper classes for data records
        public class ProductRecord
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Barcode { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal Cost { get; set; }
            public int Quantity { get; set; }
        }

        public class UserRecord
        {
            public int Id { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string WageType { get; set; } = string.Empty;
            public decimal WageAmount { get; set; }
        }

        public class ExpenseRecord
        {
            public int Id { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Category { get; set; } = string.Empty;
            public string Notes { get; set; } = string.Empty;
        }

        public class DebtRecord
        {
            public int Id { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string CustomerPhone { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public decimal PaidAmount { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        static SQLiteDatabase()
        {
            InitializeDatabase();
        }

        public static void InitializeDatabase()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                // Users table
                string usersTable = @"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL,
                    WageType TEXT,
                    WageAmount REAL,
                    IsActive INTEGER DEFAULT 1,
                    LastLogin TEXT,
                    TotalSales INTEGER DEFAULT 0,
                    DateCreated TEXT DEFAULT CURRENT_TIMESTAMP
                );";

                // Products table
                string productsTable = @"CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Barcode TEXT UNIQUE,
                    Price REAL NOT NULL,
                    Cost REAL NOT NULL,
                    Quantity INTEGER NOT NULL,
                    MinStockLevel INTEGER DEFAULT 5,
                    LastRestocked TEXT
                );";

                // Sales table
                string salesTable = @"CREATE TABLE IF NOT EXISTS Sales (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ProductId INTEGER,
                    Quantity INTEGER,
                    UnitPrice REAL,
                    TotalPrice REAL,
                    PaymentMethod TEXT,
                    SaleDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    CashierId INTEGER,
                    FOREIGN KEY(ProductId) REFERENCES Products(Id),
                    FOREIGN KEY(CashierId) REFERENCES Users(Id)
                );";

                // Debts table
                string debtsTable = @"CREATE TABLE IF NOT EXISTS Debts (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CustomerName TEXT NOT NULL,
                    CustomerPhone TEXT,
                    Amount REAL NOT NULL,
                    PaidAmount REAL DEFAULT 0,
                    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    DueDate TEXT,
                    Status TEXT DEFAULT 'Pending',
                    LastPaymentDate TEXT,
                    CashierId INTEGER,
                    Notes TEXT,
                    FOREIGN KEY(CashierId) REFERENCES Users(Id)
                );";

                // Expenses table
                string expensesTable = @"CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Category TEXT,
                    ExpenseDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    RecordedBy INTEGER,
                    Notes TEXT,
                    FOREIGN KEY(RecordedBy) REFERENCES Users(Id)
                );";

                // Requests table
                string requestsTable = @"CREATE TABLE IF NOT EXISTS Requests (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Type TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    Status TEXT DEFAULT 'Pending',
                    CreatedDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    CreatedBy INTEGER,
                    ProcessedDate TEXT,
                    ProcessedBy INTEGER,
                    Notes TEXT,
                    FOREIGN KEY(CreatedBy) REFERENCES Users(Id),
                    FOREIGN KEY(ProcessedBy) REFERENCES Users(Id)
                );";

                // CashierNotes table
                string cashierNotesTable = @"CREATE TABLE IF NOT EXISTS CashierNotes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CashierId INTEGER NOT NULL,
                    NoteDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    Notes TEXT NOT NULL,
                    FOREIGN KEY(CashierId) REFERENCES Users(Id)
                );";

                // Execute all table creation commands
                ExecuteNonQuery(usersTable, conn);
                ExecuteNonQuery(productsTable, conn);
                ExecuteNonQuery(salesTable, conn);
                ExecuteNonQuery(debtsTable, conn);
                ExecuteNonQuery(expensesTable, conn);
                ExecuteNonQuery(requestsTable, conn);
                ExecuteNonQuery(cashierNotesTable, conn);

                // Insert default admin if not exists
                string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Username='admin'";
                if ((long)ExecuteScalar(checkAdmin, conn) == 0)
                {
                    string insertAdmin = @"INSERT INTO Users (Username, Password, Role) 
                                        VALUES ('admin', 'admin2324142', 'admin')";
                    ExecuteNonQuery(insertAdmin, conn);
                }

                // Insert default cashier if not exists
                string checkCashier = "SELECT COUNT(*) FROM Users WHERE Username='cashier'";
                if ((long)ExecuteScalar(checkCashier, conn) == 0)
                {
                    string insertCashier = @"INSERT INTO Users (Username, Password, Role, WageType, WageAmount) 
                                          VALUES ('cashier', 'cashier', 'cashier', 'Hourly', 10.0)";
                    ExecuteNonQuery(insertCashier, conn);
                }
            }
        }

        private static void ExecuteNonQuery(string commandText, SQLiteConnection connection)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(commandText, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static object ExecuteScalar(string commandText, SQLiteConnection connection)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(commandText, connection))
            {
                return cmd.ExecuteScalar();
            }
        }

        // Product Methods
        public static DataTable GetProducts()
        {
            return GetDataTable("SELECT * FROM Products");
        }

        public static List<ProductRecord> GetProductList()
        {
            var products = new List<ProductRecord>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Products", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(new ProductRecord
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString() ?? string.Empty,
                            Barcode = reader["Barcode"].ToString() ?? string.Empty,
                            Price = Convert.ToDecimal(reader["Price"]),
                            Cost = Convert.ToDecimal(reader["Cost"]),
                            Quantity = Convert.ToInt32(reader["Quantity"])
                        });
                    }
                }
            }
            return products;
        }

        public static List<string> GetLowStockProducts()
        {
            var products = new List<string>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT Name FROM Products WHERE Quantity <= MinStockLevel", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        products.Add(reader["Name"].ToString() ?? string.Empty);
                    }
                }
            }
            return products;
        }

        // User Methods
        public static DataTable GetCashiers()
        {
            return GetDataTable("SELECT * FROM Users WHERE Role='cashier'");
        }

        public static UserRecord? GetUser(string username)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Users WHERE Username=@username", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new UserRecord
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Username = reader["Username"].ToString() ?? string.Empty,
                                Password = reader["Password"].ToString() ?? string.Empty,
                                Role = reader["Role"].ToString() ?? string.Empty,
                                WageType = reader["WageType"].ToString() ?? string.Empty,
                                WageAmount = Convert.ToDecimal(reader["WageAmount"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        public static void CreateUser(string username, string password, string role, string wageType, decimal wageAmount)
        {
            ExecuteNonQuery(
                @"INSERT INTO Users (Username, Password, Role, WageType, WageAmount) 
                VALUES (@username, @password, @role, @wageType, @wageAmount)",
                ("@username", username),
                ("@password", password),
                ("@role", role),
                ("@wageType", wageType),
                ("@wageAmount", wageAmount)
            );
        }

        public static void UpdateUser(string username, string password, string wageType, decimal wageAmount)
        {
            ExecuteNonQuery(
                @"UPDATE Users SET Password=@password, WageType=@wageType, WageAmount=@wageAmount 
                WHERE Username=@username",
                ("@password", password),
                ("@wageType", wageType),
                ("@wageAmount", wageAmount),
                ("@username", username)
            );
        }

        public static void DeactivateUser(string username)
        {
            ExecuteNonQuery(
                "UPDATE Users SET IsActive=0 WHERE Username=@username",
                ("@username", username)
            );
        }

        // Expense Methods
        public static DataTable GetExpenses()
        {
            return GetDataTable("SELECT * FROM Expenses");
        }

        public static ExpenseRecord? GetExpense(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Expenses WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ExpenseRecord
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Description = reader["Description"].ToString() ?? string.Empty,
                                Amount = Convert.ToDecimal(reader["Amount"]),
                                Category = reader["Category"].ToString() ?? string.Empty,
                                Notes = reader["Notes"].ToString() ?? string.Empty
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static void AddExpense(string description, decimal amount, string category, string notes)
        {
            ExecuteNonQuery(
                @"INSERT INTO Expenses (Description, Amount, Category, Notes) 
                VALUES (@desc, @amount, @category, @notes)",
                ("@desc", description),
                ("@amount", amount),
                ("@category", category),
                ("@notes", notes)
            );
        }

        public static void UpdateExpense(int id, string description, decimal amount, string category, string notes)
        {
            ExecuteNonQuery(
                @"UPDATE Expenses SET Description=@desc, Amount=@amount, 
                Category=@category, Notes=@notes WHERE Id=@id",
                ("@desc", description),
                ("@amount", amount),
                ("@category", category),
                ("@notes", notes),
                ("@id", id)
            );
        }

        public static void DeleteExpense(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM Expenses WHERE Id=@id",
                ("@id", id)
            );
        }

        // Sales and Debt Methods
        public static void RecordSale(int productId, int quantity, decimal price, string paymentMethod, string cashierUsername)
        {
            ExecuteNonQuery(
                @"INSERT INTO Sales (ProductId, Quantity, UnitPrice, TotalPrice, PaymentMethod, CashierId)
                VALUES (@productId, @quantity, @price, @total, @method, 
                        (SELECT Id FROM Users WHERE Username=@username))",
                ("@productId", productId),
                ("@quantity", quantity),
                ("@price", price),
                ("@total", price * quantity),
                ("@method", paymentMethod),
                ("@username", cashierUsername)
            );
        }

        public static DataTable GetDebts()
        {
            return GetDataTable("SELECT * FROM Debts");
        }

        public static void RecordDebt(string customerName, string phone, decimal amount, string cashierUsername)
        {
            ExecuteNonQuery(
                @"INSERT INTO Debts (CustomerName, CustomerPhone, Amount, CashierId)
                VALUES (@name, @phone, @amount, 
                        (SELECT Id FROM Users WHERE Username=@username))",
                ("@name", customerName),
                ("@phone", phone),
                ("@amount", amount),
                ("@username", cashierUsername)
            );
        }

        // Request Methods
        public static int GetPendingRequestsCount()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Requests WHERE Status='Pending'", conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void SubmitRequest(string type, string content, string username)
        {
            ExecuteNonQuery(
                @"INSERT INTO Requests (Type, Content, CreatedBy)
                VALUES (@type, @content, 
                        (SELECT Id FROM Users WHERE Username=@username))",
                ("@type", type),
                ("@content", content),
                ("@username", username)
            );
        }

        // Cashier Notes Methods
        public static DataTable GetCashierNotes()
        {
            return GetDataTable(@"SELECT u.Username, cn.NoteDate, cn.Notes 
                               FROM CashierNotes cn
                               JOIN Users u ON cn.CashierId = u.Id
                               ORDER BY cn.NoteDate DESC
                               LIMIT 30");
        }

        public static void SaveCashierNote(string username, string notes)
        {
            ExecuteNonQuery(
                @"INSERT INTO CashierNotes (CashierId, Notes)
                VALUES ((SELECT Id FROM Users WHERE Username=@username), @notes)",
                ("@username", username),
                ("@notes", notes)
            );
        }

        // Summary and Reporting
        public static DataTable GetSummaryData()
        {
            string query = @"SELECT 
                            p.Name AS Product,
                            p.Quantity AS Stock,
                            p.Price AS CurrentPrice,
                            SUM(s.Quantity) AS SoldToday,
                            SUM(s.TotalPrice) AS RevenueToday
                        FROM Products p
                        LEFT JOIN Sales s ON p.Id = s.ProductId 
                            AND DATE(s.SaleDate) = DATE('now')
                        GROUP BY p.Id";
            return GetDataTable(query);
        }

        // Helper Methods
        private static DataTable GetDataTable(string query)
        {
            var dt = new DataTable();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                using (var da = new SQLiteDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }



        // Add this public method to SQLiteDatabase.cs
        public static void ExecuteNonQuery(string query, params (string name, object value)[] parameters)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.name, param.value);
                    }
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}