using Microsoft.Data.Sqlite;
using StudySpark.Core.FileManager;
using StudySpark.Core.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.Repositories
{
    public class GitRepository
    {
        private SqliteConnection conn;

        public GitRepository()
        {
            try
            {
                // Create a new database connection:
                SqliteConnection sqlite_conn = new SqliteConnection("Data Source = ..\\..\\..\\..\\StudySpark.Core\\bin\\Debug\\net6.0\\database.db");
                // Open the connection:
                sqlite_conn.Open();
                this.conn = sqlite_conn;
                CreateTable();
            }
            catch (Exception ex) { }
        }

        private void CreateTable()
        {
            if (this.conn == null)
            {
                return;
            }

            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS GitTable (id INT, path VARCHAR(256), targetname VARCHAR(64), type VARCHAR(32))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public bool InsertData(string fullpath, string type)
        {
            if (this.conn == null)
            {
                return false;
            }

            int pos = fullpath.LastIndexOf('\\') + 1;
            fullpath = new FileInfo(fullpath).ToString();

            string path = fullpath.Substring(0, pos - 1);
            string targetname = fullpath.Substring(pos);

            List<GenericGit> files = ReadData();
            foreach (GenericGit file in files)
            {
                if ((file.TargetName).Equals(targetname))
                {
                    return false;
                }
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO GitTable (path, targetname, type, image) VALUES('{path}', '{targetname}', '{type}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;

        }

        public List<GenericGit> ReadData()
        {
            if (this.conn == null)
            {
                return new List<GenericGit>();
            }

            List<GenericGit> files = new List<GenericGit>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM GitTable";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                string dbPath = reader.GetString("path");
                string dbTargetName = reader.GetString("targetname");
                string dbType = reader.GetString("type");

                GenericGit file = new GenericGit(1, dbPath, dbTargetName, dbType);
                files.Add(file);
            }
            return files;
        }
    }
}
