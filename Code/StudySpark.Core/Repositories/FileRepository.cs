﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.SQLite;
//using System.Data.Entity;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using StudySpark.Core.FileManager;
using Microsoft.Data.Sqlite;

namespace StudySpark.Core.Repositories
{
    // Collection of logic-methods
    // for the SQLite DB
    public class FileRepository
    {
        private SqliteConnection conn;
        public FileRepository()
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
            string Createsql = "CREATE TABLE IF NOT EXISTS FileTable (id INT, path VARCHAR(256), targetname VARCHAR(64), type VARCHAR(32), image VARCHAR(64))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public bool InsertData(string fullpath, string type, string image)
        {
            if (this.conn == null)
            {
                return false;
            }

            int pos = fullpath.LastIndexOf('\\') + 1;
            fullpath = new FileInfo(fullpath).ToString();

            string path = fullpath.Substring(0, pos - 1);
            string targetname = fullpath.Substring(pos);

            List<GenericFile> files = ReadData();
            foreach (GenericFile file in files)
            {
                if ((file.TargetName).Equals(targetname))
                {
                    return false;
                }
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO FileTable (path, targetname, type, image) VALUES('{path}', '{targetname}', '{type}', '{image}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;
            
        }
        
        public List<GenericFile> ReadData()
        {
            if (this.conn == null)
            {
                return new List<GenericFile>();
            }

            List<GenericFile> files = new List<GenericFile>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM FileTable";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read())
            {
                string dbPath = reader.GetString("path");
                string dbTargetName = reader.GetString("targetname");
                string dbType = reader.GetString("type");
                string dbImage = reader.GetString("image");

                GenericFile file = new GenericFile(1, dbPath, dbTargetName, dbType, dbImage);
                files.Add(file);
            }
            return files;
        }

    }
}
