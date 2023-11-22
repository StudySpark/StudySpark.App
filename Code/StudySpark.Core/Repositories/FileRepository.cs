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
using StudySpark.Core.FileManager;

namespace StudySpark.Core.Repositories
{
    // Collection of logic-methods
    // for the SQLite DB
    public class FileRepository
    {
        private SQLiteConnection conn;
        public FileRepository()
        {
            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source = ..\\..\\..\\..\\StudySpark.Core\\bin\\Debug\\net6.0\\database.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
            sqlite_conn.Open();
            this.conn = sqlite_conn;
            CreateTable();
        }

        private void CreateTable()
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS FileTable (id INT, path VARCHAR(256), targetname VARCHAR(64))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();

        }

        public void InsertData()
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO FileTable (path, targetname) VALUES('Ditiseentest', 'test'); ";
            sqlite_cmd.ExecuteNonQuery();

        }

        public List<GenericFile> ReadData()
        {
            List<GenericFile> files = new List<GenericFile>();

            SQLiteDataReader reader;
            SQLiteCommand sqlite_cmd;
            
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM FileTable";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                int dbID = reader.GetInt16("id");
                string dbPath = reader.GetString("path");
                string dbTargetName = reader.GetString("targetname");
                string dbType = reader.GetString("type");
                string dbImage = reader.GetString("image");

                GenericFile file = new GenericFile(dbID, dbPath, dbTargetName, dbType, dbImage);
                files.Add(file);
            }
            return files;
        }

    }
}