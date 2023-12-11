using System;
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
        public bool InsertFileData(string fullpath, string type, string image)
        {
            if (DBRepository.Conn == null)
            {
                return false;
            }

            int pos = fullpath.LastIndexOf('\\') + 1;
            fullpath = new FileInfo(fullpath).ToString();

            string path = fullpath.Substring(0, pos - 1);
            string targetname = fullpath.Substring(pos);

            List<GenericFile> files = ReadFileData();
            foreach (GenericFile file in files)
            {
                if ((file.TargetName).Equals(targetname))
                {
                    return false;
                }
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Files (path, targetname, type, image) VALUES('{path}', '{targetname}', '{type}', '{image}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;

        }

        public bool InsertData(string fullpath, string extension)
        {
            string type = "";
            string image = "";

            extension = extension.ToLower();

            if (extension == "docx")
            {
                type = "WordFile";
                image = "Word.png";
            }
            else if (extension == "pptx")
            {
                type = "PowerPoint";
                image = "PowerPoint.png";
            }
            else if (extension == "xlsx")
            {
                type = "ExcelSheet";
                image = "Excel.png";
            } 
            else
            {
                type = "File";
                image = "FileIcon.png";
            }

            bool result = InsertData(fullpath, type, image);
            return result;
            
        }
        public List<GenericFile> ReadFileData()
        {

            if (DBRepository.Conn == null)
            {
                return new List<GenericFile>();
            }

            List<GenericFile> files = new List<GenericFile>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Files";

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

        public bool InsertData(string fullpath, string type, string image)
        {
            if (DBRepository.Conn == null)
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
            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Files(path, targetname, type, image) VALUES('{path}', '{targetname}', '{type}', '{image}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;

        }

        public List<GenericFile> ReadData()
        {
            if (DBRepository.Conn == null)
            {
                return new List<GenericFile>();
            }

            List<GenericFile> files = new List<GenericFile>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = DBRepository.Conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Files";

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