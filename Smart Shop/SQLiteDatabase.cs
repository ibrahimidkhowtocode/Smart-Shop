using System;
using System.Data.SQLite;
using System.Data;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Smart_Shop
{
    public static class SQLiteDatabase
    {
        private static string connectionString = "Data Source=SmartShop.db;Version=3;";

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

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private static void InitializeDatabase()
        {
            if (!File.Exists("SmartShop.db"))
            {
                SQLiteConnection.CreateFile("SmartShop.db");
            }

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                conn.Open();

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS Users (
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
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Barcode TEXT UNIQUE,
                    Price REAL NOT NULL,
                    Cost REAL NOT NULL,
                    Quantity INTEGER NOT NULL,
                    MinStockLevel INTEGER DEFAULT 5,
                    LastRestocked TEXT
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS Sales (
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
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS Debts (
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
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS DebtPayments (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    DebtId INTEGER NOT NULL,
                    Amount REAL NOT NULL,
                    PaymentDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    CashierId INTEGER,
                    FOREIGN KEY(DebtId) REFERENCES Debts(Id),
                    FOREIGN KEY(CashierId) REFERENCES Users(Id)
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS Expenses (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Description TEXT NOT NULL,
                    Amount REAL NOT NULL,
                    Category TEXT,
                    ExpenseDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    RecordedBy INTEGER,
                    Notes TEXT,
                    FOREIGN KEY(RecordedBy) REFERENCES Users(Id)
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS Requests (
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
                )", conn);

                ExecuteNonQueryWithConnection(@"CREATE TABLE IF NOT EXISTS CashierNotes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    CashierId INTEGER NOT NULL,
                    NoteDate TEXT DEFAULT CURRENT_TIMESTAMP,
                    Notes TEXT NOT NULL,
                    FOREIGN KEY(CashierId) REFERENCES Users(Id)
                )", conn);

                string checkAdmin = "SELECT COUNT(*) FROM Users WHERE Username='admin' AND Role='Admin'";
                var adminCount = ExecuteScalarWithConnection(checkAdmin, conn);
                if (adminCount != null && Convert.ToInt64(adminCount) == 0)
                {
                    ExecuteNonQueryWithConnection(
                        @"INSERT INTO Users (Username, Password, Role, IsActive) 
                        VALUES ('admin', @password, 'Admin', 1)",
                        conn,
                        ("@password", HashPassword("Admin@1234"))
                    );

                    MessageBox.Show("Default admin created:\nUsername: admin\nPassword: Admin@1234",
                        "Database Initialized", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private static void ExecuteNonQueryWithConnection(string commandText, SQLiteConnection connection)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(commandText, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static void ExecuteNonQueryWithConnection(string commandText, SQLiteConnection connection, params (string name, object value)[] parameters)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(commandText, connection))
            {
                foreach (var param in parameters)
                {
                    cmd.Parameters.AddWithValue(param.name, param.value);
                }
                cmd.ExecuteNonQuery();
            }
        }

        private static object? ExecuteScalarWithConnection(string commandText, SQLiteConnection connection)
        {
            using (SQLiteCommand cmd = new SQLiteCommand(commandText, connection))
            {
                return cmd.ExecuteScalar();
            }
        }

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

        public static bool ValidateUser(string username, string password)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                string hashedInput = HashPassword(password);

                using (var cmd = new SQLiteCommand(
                    "SELECT Password FROM Users WHERE Username=@username AND IsActive=1", conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    var result = cmd.ExecuteScalar();

                    if (result == null)
                    {
                        return false;
                    }

                    string storedHash = result.ToString() ?? string.Empty;
                    return storedHash.Equals(hashedInput);
                }
            }
        }

        public static DataTable GetProducts()
        {
            return GetDataTable("SELECT * FROM Products");
        }

        public static ProductRecord? GetProduct(int id)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Products WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ProductRecord
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Barcode = reader["Barcode"]?.ToString() ?? string.Empty,
                                Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m,
                                Cost = reader["Cost"] != DBNull.Value ? Convert.ToDecimal(reader["Cost"]) : 0m,
                                Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static ProductRecord? GetProductByBarcode(string barcode)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Products WHERE Barcode=@barcode", conn))
                {
                    cmd.Parameters.AddWithValue("@barcode", barcode);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new ProductRecord
                            {
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Barcode = reader["Barcode"]?.ToString() ?? string.Empty,
                                Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0m,
                                Cost = reader["Cost"] != DBNull.Value ? Convert.ToDecimal(reader["Cost"]) : 0m,
                                Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToInt32(reader["Quantity"]) : 0
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static void AddProduct(string name, string barcode, decimal price, decimal cost, int quantity)
        {
            ExecuteNonQuery(
                @"INSERT INTO Products (Name, Barcode, Price, Cost, Quantity) 
                VALUES (@name, @barcode, @price, @cost, @quantity)",
                ("@name", name),
                ("@barcode", barcode),
                ("@price", price),
                ("@cost", cost),
                ("@quantity", quantity)
            );
        }

        public static void UpdateProduct(int id, string name, string barcode, decimal price, decimal cost, int quantity)
        {
            ExecuteNonQuery(
                @"UPDATE Products SET 
                    Name=@name, 
                    Barcode=@barcode, 
                    Price=@price, 
                    Cost=@cost, 
                    Quantity=@quantity 
                WHERE Id=@id",
                ("@name", name),
                ("@barcode", barcode),
                ("@price", price),
                ("@cost", cost),
                ("@quantity", quantity),
                ("@id", id)
            );
        }

        public static void DeleteProduct(int id)
        {
            ExecuteNonQuery(
                "DELETE FROM Products WHERE Id=@id",
                ("@id", id)
            );
        }

        public static DataTable GetCashiers()
        {
            return GetDataTable(@"SELECT 
                Id, 
                Username, 
                WageType, 
                WageAmount,
                LastLogin,
                TotalSales
                FROM Users 
                WHERE Role='Cashier' AND IsActive=1
                ORDER BY Username");
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
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                Username = reader["Username"]?.ToString() ?? string.Empty,
                                Password = reader["Password"]?.ToString() ?? string.Empty,
                                Role = reader["Role"]?.ToString() ?? string.Empty,
                                WageType = reader["WageType"]?.ToString() ?? string.Empty,
                                WageAmount = reader["WageAmount"] != DBNull.Value ? Convert.ToDecimal(reader["WageAmount"]) : 0m
                            };
                        }
                    }
                }
            }
            return null;
        }

        public static void CreateUser(string username, string password, string role, string wageType, decimal wageAmount)
        {
            string hashedPassword = HashPassword(password);
            ExecuteNonQuery(
                @"INSERT INTO Users (Username, Password, Role, WageType, WageAmount, IsActive) 
                VALUES (@username, @password, @role, @wageType, @wageAmount, 1)",
                ("@username", username),
                ("@password", hashedPassword),
                ("@role", role),
                ("@wageType", wageType),
                ("@wageAmount", wageAmount)
            );
        }

        public static void UpdateUser(string username, string password, string wageType, decimal wageAmount)
        {
            string hashedPassword = HashPassword(password);
            ExecuteNonQuery(
                @"UPDATE Users SET 
                    Password=@password, 
                    WageType=@wageType, 
                    WageAmount=@wageAmount 
                WHERE Username=@username",
                ("@password", hashedPassword),
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

        public static DataTable GetExpenses()
        {
            return GetDataTable("SELECT * FROM Expenses ORDER BY ExpenseDate DESC");
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
                                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                Amount = reader["Amount"] != DBNull.Value ? Convert.ToDecimal(reader["Amount"]) : 0m,
                                Category = reader["Category"]?.ToString() ?? string.Empty,
                                Notes = reader["Notes"]?.ToString() ?? string.Empty
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
                @"UPDATE Expenses SET 
                    Description=@desc, 
                    Amount=@amount, 
                    Category=@category, 
                    Notes=@notes 
                WHERE Id=@id",
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

        public static void RecordSale(int productId, int quantity, decimal price, string paymentMethod, string cashierUsername)
        {
            ExecuteNonQuery(
                @"INSERT INTO Sales (ProductId, Quantity, UnitPrice, TotalPrice, PaymentMethod, CashierId)
                VALUES (@productId, @quantity, @price, @total, @method, 
                        (SELECT Id FROM Users WHERE Username=@username));
                
                UPDATE Products SET Quantity = Quantity - @quantity WHERE Id=@productId",
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
            return GetDataTable(@"SELECT d.Id, d.CustomerName, d.CustomerPhone, 
                               d.Amount, d.PaidAmount, d.Amount - d.PaidAmount AS Remaining,
                               d.Status, u.Username AS Cashier, d.CreatedDate
                               FROM Debts d
                               JOIN Users u ON d.CashierId = u.Id
                               ORDER BY d.CreatedDate DESC");
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

        public static void RecordDebtPayment(int debtId, decimal amount, string cashierUsername)
        {
            ExecuteNonQuery(
                @"UPDATE Debts SET 
                    PaidAmount = PaidAmount + @amount,
                    Status = CASE WHEN (Amount - (PaidAmount + @amount)) <= 0 THEN 'Paid' ELSE 'Partial' END,
                    LastPaymentDate = CURRENT_TIMESTAMP
                WHERE Id = @debtId;
                
                INSERT INTO DebtPayments (DebtId, Amount, PaymentDate, CashierId)
                VALUES (@debtId, @amount, CURRENT_TIMESTAMP, 
                        (SELECT Id FROM Users WHERE Username=@username))",
                ("@amount", amount),
                ("@debtId", debtId),
                ("@username", cashierUsername)
            );
        }

        public static int GetPendingRequestsCount()
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Requests WHERE Status='Pending'", conn))
                {
                    var result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public static List<string> GetLowStockProducts()
        {
            var lowStockItems = new List<string>();
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(
                    "SELECT Name FROM Products WHERE Quantity <= MinStockLevel", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["Name"] != DBNull.Value)
                            {
                                lowStockItems.Add(reader["Name"].ToString() ?? string.Empty);
                            }
                        }
                    }
                }
            }
            return lowStockItems;
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

        public static DataTable GetSummaryData()
        {
            return GetDataTable(@"SELECT 
                                p.Name AS Product,
                                p.Quantity AS Stock,
                                p.Price AS CurrentPrice,
                                COALESCE(SUM(s.Quantity), 0) AS SoldToday,
                                COALESCE(SUM(s.TotalPrice), 0) AS RevenueToday
                            FROM Products p
                            LEFT JOIN Sales s ON p.Id = s.ProductId 
                                AND DATE(s.SaleDate) = DATE('now')
                            GROUP BY p.Id");
        }

        public static DataTable GetDataTable(string query)
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
    }
}