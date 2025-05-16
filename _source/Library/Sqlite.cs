using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace bugfish_nuke.Library
{
    internal class Sqlite
    {
        private SQLiteConnection sqliteConnection;

        public Sqlite(string databasePath)
        {
            string connectionString = $"Data Source={databasePath};Version=3;";
            sqliteConnection = new SQLiteConnection(connectionString);
            sqliteConnection.Open();
        }

        public void CreateTable(string createTableQuery)
        {
            using (var command = new SQLiteCommand(createTableQuery, sqliteConnection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void InsertData(string insertQuery, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(insertQuery, sqliteConnection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }

        public void UpdateData(string updateQuery, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(updateQuery, sqliteConnection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }

        public void DeleteData(string deleteQuery, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(deleteQuery, sqliteConnection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
        }

        public DataTable GetDataTable(string selectQuery, params SQLiteParameter[] parameters)
        {
            using (var command = new SQLiteCommand(selectQuery, sqliteConnection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                using (var adapter = new SQLiteDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        public void Dispose()
        {
            if (sqliteConnection != null)
            {
                sqliteConnection.Close();
                sqliteConnection.Dispose();
            }
        }
    }
}
