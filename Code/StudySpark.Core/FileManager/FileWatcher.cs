using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StudySpark.Core.FileManager {
    public class FileWatcher {

        public event EventHandler? FileChangedOnFSEvent;

        private List<GenericFile> filesToWatch = new List<GenericFile>();
        private bool watcherRunning = false;
        private Thread watcherThread;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);

        int index = 0;

        public FileWatcher() {
            watcherThread = new Thread(Tick);
            watcherRunning = true;
            resetEvent.Set();
            watcherThread.Start(index);
        }

        public void SetFilesToWatch(List<GenericFile> files) {
            watcherRunning = false;
            resetEvent.Reset();
            watcherThread.Join();

            filesToWatch.Clear();
            foreach (var item in files) {
                filesToWatch.Add(item);
            }

            watcherThread = new Thread(Tick);
            watcherRunning = true;
            index++;
            resetEvent.Set();
            watcherThread.Start(index);
        }

        private void Tick(object? i) {
            int? j = (int)i;

            while (watcherRunning) {
                Debug.WriteLine("==========");
                filesToWatch.ForEach(item => {
                    Debug.WriteLine($"{j}: {item.TargetName}");
                });
                Debug.WriteLine("==========");
                Debug.WriteLine("");

                if (!resetEvent.WaitOne(1000)) {
                    break; // Exit the loop if the event is signaled
                }

                Thread.Sleep(1000);
            }
        }
    }
}
