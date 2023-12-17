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
        private static SqliteConnection conn;   
        public static SqliteConnection Conn {
            get {
                if (conn == null)
                {
                    Logger.Info("Creating databse connection");

                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StudySpark"));

                    string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StudySpark", "database.db");

                    Logger.Info($"Databse path: {databasePath}");

                    conn = new SqliteConnection($"Data Source={databasePath}");
                    conn.Open();

                    string commandPath = "initDB.txt";
                    if (!File.Exists(commandPath)) {
                        commandPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "initDB.txt");
                    }
                    Logger.Info($"SQL init file path: {commandPath}");

                    SqliteCommand sqlite_cmd = conn.CreateCommand();
                    sqlite_cmd.CommandText = File.ReadAllText(commandPath);
                    sqlite_cmd.ExecuteNonQuery();


                    string connCreatedSuccessfullyMessage = conn != null ? "Yes" : "No";
                    Logger.Info($"Connection created: {connCreatedSuccessfullyMessage}");
                }
                return conn;
            }
        }

        public bool InsertGrade(GradeElement gradeElement) {
            if (Conn == null) {
                return false;
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = Conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO Grades (coursename, coursecode, testdate, semester, ecs, grade) VALUES ('{gradeElement.CourseName}', '{gradeElement.CourseCode}', '{gradeElement.TestDate}', '{gradeElement.Semester}', {gradeElement.ECs}, '{gradeElement.Grade}');";
            sqlite_cmd.ExecuteNonQuery();
            return true;
        }

        public void CreateUser(string username, string password) {
            byte[] key = Encoding.UTF8.GetBytes("1jlSTUDYSPARKbzJPAuhjXAQluf/e5e4");
            byte[] iv = Encoding.UTF8.GetBytes("420694206942069F");

            string encryptedString = Encryption.EncryptString(password, key, iv);

            string deletesql = "DELETE FROM Users;";
            SqliteCommand deleteCmd = new SqliteCommand(deletesql, Conn);
            deleteCmd.ExecuteNonQuery();

            string createsql = "INSERT INTO Users (Username, Password) VALUES(@Username, @Password)";
            SqliteCommand insertSQL = new SqliteCommand(createsql, Conn);
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
            SqliteCommand cmd = new SqliteCommand(query, Conn);
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
            if (Conn == null) {
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
            sqlite_cmd = Conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT INTO GitTable (path, targetname, type) VALUES('{path}', '{targetname}', '{type}'); ";
            sqlite_cmd.ExecuteNonQuery();
            return true;

        }

        

        public List<GradeElement> ReadGradesData() {
            if (Conn == null) {
                return new List<GradeElement>();
            }

            List<GradeElement> gradeElements = new List<GradeElement>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = Conn.CreateCommand();
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
            if (Conn == null) {
                return new List<GenericGit>();
            }

            List<GenericGit> repos = new List<GenericGit>();

            SqliteDataReader reader;
            SqliteCommand sqlite_cmd;

            sqlite_cmd = Conn.CreateCommand();
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
            SqliteCommand deleteCmd = new SqliteCommand(deletesql, Conn);
            deleteCmd.ExecuteNonQuery();
        }

        public bool DeleteFileData(string path, string targetname)
        {
            SqliteCommand sqlite_cmd;
            sqlite_cmd = Conn.CreateCommand();
            sqlite_cmd.CommandText = $"DELETE FROM Files WHERE path = '{path}' AND targetname = '{targetname}'; ";
            sqlite_cmd.ExecuteNonQuery();
            return true;
        }
    }
}