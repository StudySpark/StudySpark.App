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
                    conn = new SqliteConnection("Data Source = ..\\..\\..\\..\\StudySpark.Core\\bin\\Debug\\net6.0\\database.db");
                    conn.Open();

                    string command = File.ReadAllText("initDB.txt");
                    SqliteCommand sqlite_cmd = conn.CreateCommand();
                    sqlite_cmd.CommandText = command;
                    sqlite_cmd.ExecuteNonQuery();
                }
                return conn;
            }
        }

        public bool InsertGrade(GradeElement gradeElement) {
            if (Conn == null) {
                return false;
            }

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = $"INSERT OR REPLACE INTO Grades (coursename, coursecode, testdate, semester, ecs, grade) VALUES ('{gradeElement.CourseName}', '{gradeElement.CourseCode}', '{gradeElement.TestDate}', '{gradeElement.Semester}', {gradeElement.ECs}, '{gradeElement.Grade}');";
            sqlite_cmd.ExecuteNonQuery();
            return true;
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
            sqlite_cmd = conn.CreateCommand();
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
            if (Conn == null) {
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
    }
}