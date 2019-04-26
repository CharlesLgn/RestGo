using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace RestManDataAccess
{
    public static class DataAccess
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();

                //string foo1 = "DROP TABLE HISTORY;";

                String tableCommandBasic = "CREATE TABLE IF NOT EXISTS BASICTOKEN (ID INTEGER PRIMARY KEY, USERNAME TEXT NOT NULL, PASSWORD TEXT NOT NULL, DATE TEXT, LABEL TEXT)";
                String tableCommandCustom = "CREATE TABLE IF NOT EXISTS CUSTOMTOKEN (ID INTEGER PRIMARY KEY, USERNAME TEXT NOT NULL, PASSWORD TEXT NOT NULL, DATE TEXT, LABEL TEXT )";
                String tableConfig = "CREATE TABLE IF NOT EXISTS CONFIG (ID INTEGER PRIMARY KEY, TYPE TEXT NOT NULL, URL TEXT NOT NULL, BODY TEXT NOT NULL, LABEL TEXT NOT NULL)";
                String tableHistory = "CREATE TABLE IF NOT EXISTS HISTORY (ID INTEGER PRIMARY KEY, TYPE TEXT NOT NULL, URL TEXT NOT NULL, DATE TEXT NOT NULL, BODY TEXT NOT NULL)";

                //SqliteCommand foo1b1 = new SqliteCommand(foo1, db);

                SqliteCommand createTableBasic = new SqliteCommand(tableCommandBasic, db);
                SqliteCommand createTableCustom = new SqliteCommand(tableCommandCustom, db);
                SqliteCommand createTableConfig = new SqliteCommand(tableConfig, db);
                SqliteCommand createTableHistory = new SqliteCommand(tableHistory, db);

                //foo1b1.ExecuteReader();

                createTableBasic.ExecuteReader();
                createTableCustom.ExecuteReader();
                createTableConfig.ExecuteReader();
                createTableHistory.ExecuteReader();
            }
        }

        public static void AddData(string table, string username, string password, string date, string label)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO " + table + " VALUES (NULL, @EntryUsername, @EntryPassword, @EntryDate, @EntryLabel);";
                insertCommand.Parameters.AddWithValue("@EntryUsername", username);
                insertCommand.Parameters.AddWithValue("@EntryPassword", password);
                insertCommand.Parameters.AddWithValue("@EntryDate", date);
                insertCommand.Parameters.AddWithValue("@EntryLabel", label);
                insertCommand.ExecuteReader();
                db.Close();
            }
        }

        public static List<string[]> GetData(string table)
        {
            List<string[]> entries = new List<string[]>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from " + table + " ORDER BY ID DESC", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    string id = query.GetString(0);
                    string username = query.GetString(1);
                    string password = query.GetString(2);
                    string date = query.GetString(3);
                    string label = query.GetString(4);
                    string[] res = { id, username, password, date, label };
                    entries.Add(res);
                }
                
                db.Close();
            }

            return entries;
        }

        public static string[] GetByID(string table, string id)
        {
            List<string[]> entries = new List<string[]>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from " + table + " WHERE ID = " + id, db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    string val1 = query.GetString(0);
                    string val2 = query.GetString(1);
                    string val3 = query.GetString(2);
                    string val4 = query.GetString(3);
                    string val5 = query.GetString(4);
                    string[] res = { val1, val2, val3, val4, val5 };
                    entries.Add(res);
                }

                db.Close();
            }

            return entries[0];
        }

        public static List<string> GetByIDConfig(int id)
        {
            List<string> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT TYPE, URL, BODY from CONFIG WHERE ID = " + id, db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    string type = query.GetString(0);
                    string url = query.GetString(1);
                    string body = query.GetString(2);
                    string res = type + '|' + url + '|' + body;
                    entries.Add(res);
                }

                db.Close();
            }

            return entries;
        }

        public static List<string> GetByIDAuthorization(string table, int id)
        {
            List<string> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT USERNAME, PASSWORD from " + table + " WHERE ID = " + id, db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    string username = query.GetString(0);
                    string password = query.GetString(1);
                    string res = username + '|' + password;
                    entries.Add(res);
                }

                db.Close();
            }

            return entries;
        }

        public static void DeleteByID(string table, string id)
        {
            //List<string> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                    ("DELETE from " + table + " WHERE ID = " + id, db);

                SqliteDataReader query = selectCommand.ExecuteReader();
                db.Close();
            }
        }

        public static void DeleteAllData(string table)
        {
            //List<string> entries = new List<string>();

            using (SqliteConnection db =
                new SqliteConnection("Filename=RestManDB.db"))
            {
                db.Open();
                SqliteCommand selectCommand = new SqliteCommand
                    ("DELETE from " + table, db);

                SqliteDataReader query = selectCommand.ExecuteReader();
                db.Close();
            }
        }
    }
}
