using System;
using System.Data;
using System.Data.SQLite;

namespace Smart_Shop
{
    public class SQLiteDatabase : IDisposable
    {
        private readonly SQLiteConnection connection;

        public SQLiteDatabase(string dbPath)
        {
            connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS products (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL,
                    price REAL NOT NULL,
                    barcode TEXT UNIQUE,
                    stock INTEGER DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS debts (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    customer_name TEXT NOT NULL,
                    amount REAL NOT NULL,
                    paid_amount REAL DEFAULT 0,
                    date TEXT DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    action TEXT NOT NULL,
                    details TEXT,
                    user TEXT NOT NULL,
                    timestamp TEXT DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS expenses (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    description TEXT NOT NULL,
                    amount REAL NOT NULL,
                    date TEXT DEFAULT CURRENT_TIMESTAMP
                );

                CREATE TABLE IF NOT EXISTS cashiers (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT UNIQUE NOT NULL,
                    password TEXT NOT NULL,
                    wage_type TEXT CHECK(wage_type IN ('monthly', 'hourly')),
                    wage_rate REAL,
                    last_login TEXT,
                    sales_count INTEGER DEFAULT 0,
                    notes TEXT
                );
            ");
        }

        public DataTable GetDataTable(string query, params SQLiteParameter[] parameters)
        {
            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddRange(parameters);
            using var da = new SQLiteDataAdapter(cmd);
            DataTable dt = new();
            da.Fill(dt);
            return dt;
        }

        public void ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using var cmd = new SQLiteCommand(query, connection);
            cmd.Parameters.AddRange(parameters);
            cmd.ExecuteNonQuery();
        }

        public void LogHistory(string action, string details, string user)
        {
            ExecuteNonQuery(
                "INSERT INTO history (action, details, user) VALUES (@action, @details, @user)",
                new SQLiteParameter("@action", action),
                new SQLiteParameter("@details", details),
                new SQLiteParameter("@user", user)
            );
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
        }
    }
}