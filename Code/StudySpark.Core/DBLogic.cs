using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Entity;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace StudySpark.Core
{
    // Collection of logic-methods
    // for the SQLite DB
    public class DBLogic
    {

        public static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source = ..\\..\\..\\..\\StudySpark.Core\\bin\\Debug\\net6.0\\database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            sqlite_conn.Open();
            return sqlite_conn;
        
        }

        public static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS FileTable (id INT, path VARCHAR(256), targetname VARCHAR(64))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();

        }

        public static void InsertData(SQLiteConnection conn)
        {

            CreateTable(conn);
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO FileTable (path, targetname) VALUES('Ditiseentest', 'test'); ";
            sqlite_cmd.ExecuteNonQuery();

        }

        public static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader reader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM FileTable";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                string myreader = reader.GetString(0);
                Console.WriteLine(myreader);
            }
        }

    }
}
