using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core {
    public class Logger {

        private static StreamWriter? _writer;
        private static StreamWriter? writer {
            get {
                if (_writer == null) {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StudySpark", "logs"));

                    DateTime currentDateTime = DateTime.Now;
                    string fileName = $"LogFile_{currentDateTime:yyyyMMdd_HHmmss}.txt";
                    string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StudySpark", "logs", fileName);
                    _writer = new StreamWriter(filePath, true);
                    _writer.AutoFlush = true;
                }
                return _writer;
            }
        }

        private static void Log(string level, string message) {
            DateTime currentDateTime = DateTime.Now;
            writer?.WriteLine($"[{level} | {currentDateTime:HH:mm:ss}] {message}");
        }

        public static void Info(string message) {
            Log("INFO", message);
        }

        public static void Warning(string message) {
            Log("WARN", message);
        }

        public static void Error(string message) {
            Log("ERROR", message);
        }
    }
}
