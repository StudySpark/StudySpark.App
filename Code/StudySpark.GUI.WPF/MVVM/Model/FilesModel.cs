using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using StudySpark.Core;
using StudySpark.Core.FileManager;

namespace StudySpark.GUI.WPF.MVVM.Model {
    class FilesModel  {

        //public event PropertyChangedEventHandler? PropertyChanged;

        //private List<File> files = new List<File>();

        //public List<File> Files { get { return files; } }

        public static void AddFile() {

            SQLiteConnection sqlite_conn = DBLogic.CreateConnection();
            DBLogic.InsertData(sqlite_conn);

        }

        public static void RemoveFile() {

            SQLiteConnection sqlite_conn = DBLogic.CreateConnection();
            DBLogic.InsertData(sqlite_conn);

        }

        public static void ReadFromDatabase()
        {

            SQLiteConnection sqlite_conn = DBLogic.CreateConnection();
            DBLogic.ReadData(sqlite_conn);

        }

        //protected virtual void OnPropertyChanged(string propertyName) {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
