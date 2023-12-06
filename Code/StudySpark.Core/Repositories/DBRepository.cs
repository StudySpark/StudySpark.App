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
using StudySpark.Core.Grades;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using StudySpark.Core.Generic;

namespace StudySpark.Core.Repositories {
    // Collection of logic-methods
    // for the SQLite DB
    public class DBRepository {
        private SqliteConnection conn;
        public DBRepository() {
            try {
                // Create a new database connection:
                SqliteConnection sqlite_conn = new SqliteConnection("Data Source = ..\\..\\..\\..\\StudySpark.Core\\bin\\Debug\\net6.0\\database.db");
                // Open the connection:
                sqlite_conn.Open();
                this.conn = sqlite_conn;

                CreateFileTable();
                CreateGradesTable();
                CreateUsersTable();
                CreateGitTable();
            } catch (Exception ex) { }
        }

        private void CreateFileTable() {
            if (this.conn == null) {
                return;
            }

            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS FileTable (id INT, path VARCHAR(256), targetname VARCHAR(64), type VARCHAR(32), image VARCHAR(64))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        private void CreateGradesTable() {
            if (this.conn == null) {
                return;
            }

            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS Grades (coursename VARCHAR(64), coursecode VARCHAR(64) PRIMARY KEY, testdate VARCHAR(32), semester VARCHAR(32), ecs INT, grade VARCHAR(5))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        private void CreateUsersTable() {
            if (this.conn == null) {
                return;
            }
            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS Users (username VARCHAR(256), password VARCHAR(64))";

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        private void CreateGitTable() {
            if (this.conn == null) {
                return;
            }

            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS GitTable (id INT, path VARCHAR(256), targetname VARCHAR(64), type VARCHAR(32))";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
        }

        public bool InsertFileData(string fullpath, string type, string image) {
            if (this.conn == null) {
                return false;
            }

            int pos = fullpath.LastIndexOf('\\') + 1;
            fullpath = new FileInfo(fullpath).ToString();

            string path = fullpath.Substring(0, pos - 1);
            string targetname = fullpath.Substring(pos);

            List<GenericFile> files = ReadFileData();
            foreach (GenericFile file in files) {
                if ((file.TargetName).Equals(targetname)) {
                    return false;
                }
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO FileTable (path, targetname, type, image) VALUES('{path}', '{targetname}', '{type}', '{image}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;

        }

        public bool InsertGrade(GradeElement gradeElement) {
            if (this.conn == null) {
                return false;
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Grades (coursename, coursecode, testdate, semester, ecs, grade) VALUES ('{gradeElement.CourseName}', '{gradeElement.CourseCode}', '{gradeElement.TestDate}', '{gradeElement.Semester}', {gradeElement.ECs}, '{gradeElement.Grade}');";
            sqlite_cmd.ExecuteNonQuery();
            return true;
        }

        public void CreateUser(string username, string password) {
            byte[] key = Encoding.UTF8.GetBytes("1jlSTUDYSPARKbzJPAuhjXAQluf/e5e4");
            byte[] iv = Encoding.UTF8.GetBytes("420694206942069F");

            string encryptedString = Encryption.EncryptString(password, key, iv);

            string deletesql = "DELETE FROM Users;";
            SqliteCommand deleteCmd = new SqliteCommand(deletesql, conn);
            deleteCmd.ExecuteNonQuery();

            string createsql = "INSERT INTO Users (Username, Password) VALUES(@Username, @Password)";
            SqliteCommand insertSQL = new SqliteCommand(createsql, conn);
            insertSQL.Parameters.Add(new SqliteParameter("@Username", username));
            insertSQL.Parameters.Add(new SqliteParameter("@Password", encryptedString));
            try {
                insertSQL.ExecuteNonQuery();
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public GenericUser? GetUser() {
            byte[] key = Encoding.UTF8.GetBytes("1jlSTUDYSPARKbzJPAuhjXAQluf/e5e4");
            byte[] iv = Encoding.UTF8.GetBytes("420694206942069F");

            string query = "SELECT * FROM Users";
            SqliteCommand cmd = new SqliteCommand(query, conn);
            try {
                SqliteDataReader reader = cmd.ExecuteReader();
                GenericUser user = new GenericUser();
                while (reader.Read()) {
                    user.Username = reader.GetString(0);
                    user.Password = Encryption.DecryptString(reader.GetString(1), key, iv);
                }
                return user;
            } catch {
                return null;
            }
        }

        public bool InsertGitData(string fullpath, string type) {
            if (this.conn == null) {
                return false;
            }

            int pos = fullpath.LastIndexOf('\\') + 1;
            fullpath = new FileInfo(fullpath).ToString();

            string path = fullpath.Substring(0, pos - 1);
            string targetname = fullpath.Substring(pos);

            List<GenericGit> repos = ReadGitData();
            foreach (GenericGit repo in repos) {
                if ((repo.TargetName).Equals(targetname)) {
                    return false;
                }
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO GitTable (path, targetname, type) VALUES('{path}', '{targetname}', '{type}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;

        }

        public List<GenericFile> ReadFileData() {
            if (this.conn == null) {
                return new List<GenericFile>();
            }

            List<GenericFile> files = new List<GenericFile>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM FileTable";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read()) {
                string dbPath = reader.GetString("path");
                string dbTargetName = reader.GetString("targetname");
                string dbType = reader.GetString("type");
                string dbImage = reader.GetString("image");

                GenericFile file = new GenericFile(1, dbPath, dbTargetName, dbType, dbImage);
                files.Add(file);
            }
            return files;
        }

        public List<GradeElement> ReadGradesData() {
            if (this.conn == null) {
                return new List<GradeElement>();
            }

            List<GradeElement> gradeElements = new List<GradeElement>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Grades";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read()) {
                GradeElement gradeElement = new GradeElement();
                gradeElement.CourseName = reader.GetString("coursename");
                gradeElement.CourseCode = reader.GetString("coursecode");
                gradeElement.TestDate = reader.GetString("testdate");
                gradeElement.Semester = reader.GetString("semester");
                gradeElement.ECs = reader.GetString("ecs");
                gradeElement.Grade = reader.GetString("grade");

                gradeElements.Add(gradeElement);
            }

            return gradeElements;
        }

        public List<GenericGit> ReadGitData() {
            if (this.conn == null) {
                return new List<GenericGit>();
            }

            List<GenericGit> repos = new List<GenericGit>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM GitTable";

            reader = sqlite_cmd.ExecuteReader();
            while (reader.Read()) {
                string dbPath = reader.GetString("path");
                string dbTargetName = reader.GetString("targetname");
                string dbType = reader.GetString("type");

                GenericGit repo = new GenericGit(1, dbPath, dbTargetName, dbType);
                repos.Add(repo);
            }
            return repos;
        }

        public void ClearGradesData() {
            string deletesql = "DELETE FROM Grades;";
            SqliteCommand deleteCmd = new SqliteCommand(deletesql, conn);
            deleteCmd.ExecuteNonQuery();
        }
    }
}