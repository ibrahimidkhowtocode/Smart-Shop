using System;
using System.Data;
using System.Data.SQLite;

namespace Smart_Shop
{
    public class SQLiteDatabase : IDisposable
    {
        private readonly SQLiteConnection connection;
        private readonly string databasePath;

        public SQLiteDatabase(string dbPath = "SmartShop.db")
        {
            databasePath = dbPath;
            connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            connection.Open();

            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Role TEXT NOT NULL
                )");

            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Barcode TEXT NOT NULL UNIQUE,
                    Name TEXT NOT NULL,
                    PurchasePrice REAL NOT NULL,
                    SalePrice REAL NOT NULL,
                    Quantity INTEGER NOT NULL,
                    MinStockLevel INTEGER NOT NULL
                )");
        }

        public DataTable GetDataTable(string query, params SQLiteParameter[] parameters)
        {
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddRange(parameters);
            using var adapter = new SQLiteDataAdapter(command);
            var dt = new DataTable();
            adapter.Fill(dt);
            return dt;
        }

        public int ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using var command = new SQLiteCommand(query, connection);
            command.Parameters.AddRange(parameters);
            return command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            connection?.Close();
            connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}