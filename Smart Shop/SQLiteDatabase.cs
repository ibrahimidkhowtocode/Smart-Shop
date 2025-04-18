using System;
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Smart_Shop
{
    public static class SQLiteDatabase
    {
        private static string connectionString = "Data Source=SmartShop.db;Version=3;";

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
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Products";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public static List<string> GetLowStockProducts()
        {
            var lowStockProducts = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT Name FROM Products 
                                 WHERE Quantity <= MinStockLevel";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lowStockProducts.Add(reader["Name"].ToString());
                        }
                    }
                }
            }
            return lowStockProducts;
        }

        // User Methods
        public static bool ValidateUser(string username, string password)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username=@username AND Password=@password";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // Debt Methods
        public static DataTable GetDebts()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Debts";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // Expense Methods
        public static DataTable GetExpenses()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Expenses";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // Request Methods
        public static int GetPendingRequestsCount()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Requests WHERE Status='Pending'";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        // Cashier Notes Methods
        public static void SaveCashierNote(string username, string notes)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO CashierNotes (CashierId, Notes) 
                                VALUES ((SELECT Id FROM Users WHERE Username=@username), @notes)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@notes", notes);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Add these to SQLiteDatabase.cs
        public static void RecordSale(int productId, int quantity, decimal price, string paymentMethod, string cashierUsername)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Sales (ProductId, Quantity, UnitPrice, TotalPrice, PaymentMethod, CashierId)
                        VALUES (@productId, @quantity, @price, @total, @method, 
                                (SELECT Id FROM Users WHERE Username=@username))";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@total", price * quantity);
                    cmd.Parameters.AddWithValue("@method", paymentMethod);
                    cmd.Parameters.AddWithValue("@username", cashierUsername);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void RecordDebt(string customerName, string phone, decimal amount, string cashierUsername)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Debts (CustomerName, CustomerPhone, Amount, CashierId)
                        VALUES (@name, @phone, @amount, 
                                (SELECT Id FROM Users WHERE Username=@username))";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", customerName);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@username", cashierUsername);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void SubmitRequest(string type, string content, string username)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Requests (Type, Content, CreatedBy)
                        VALUES (@type, @content, 
                                (SELECT Id FROM Users WHERE Username=@username))";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static DataTable GetSummaryData()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
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
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        // Add all other necessary database methods following the same pattern

        public static DataTable GetCashierNotes()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string query = @"SELECT u.Username, cn.NoteDate, cn.Notes 
                                 FROM CashierNotes cn
                                 JOIN Users u ON cn.CashierId = u.Id
                                 ORDER BY cn.NoteDate DESC
                                 LIMIT 30";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }
    }
}