using StudySpark.Core.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StudySpark.GUI.WPF.Core {
    public class SystemFileHandler {

        public static RoutedEventHandler CreateClickOpenHandler(GenericFile file) {
            return (sender, args) => {
                if (args.OriginalSource is Button clickedButton && clickedButton.Tag is string folderPath && file.TargetName is string fileName) {

                    string buttonFilePath = System.IO.Path.Combine(folderPath, fileName);

                    // Logic to run the file using the buttonFilePath
                    try {
                        using (System.Diagnostics.Process process = new System.Diagnostics.Process()) {
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo {
                                FileName = "cmd.exe",
                                Arguments = $"/c start \"\" \"{buttonFilePath}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            };

                            process.StartInfo = startInfo;
                            process.Start();
                        }
                    } catch (Exception ex) {
                        System.Windows.MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            };
        }

        public static RoutedEventHandler CreateClickRawOpenHandler(string path) {
            return (sender, args) => {
                if (args.OriginalSource is Button clickedButton) {
                    // Logic to run the file using the buttonFilePath
                    try {
                        using (System.Diagnostics.Process process = new System.Diagnostics.Process()) {
                            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo {
                                FileName = "cmd.exe",
                                Arguments = $"/c start \"\" \"{path}\"",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            };

                            process.StartInfo = startInfo;
                            process.Start();
                        }
                    } catch (Exception ex) {
                        System.Windows.MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            };
        }
    }
}
