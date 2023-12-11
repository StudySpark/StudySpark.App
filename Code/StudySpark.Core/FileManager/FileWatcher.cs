using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace StudySpark.Core.FileManager {
    public class FileWatcher {

        public event EventHandler? FileChangedOnFSEvent;

        private List<GenericFile> filesToWatch = new List<GenericFile>();
        private bool watcherRunning = false;
        private Thread? watcherThread;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);
        private static bool globalStopRequested = false;
        private Dictionary<string, bool> fileExistsBuffer = new Dictionary<string, bool>();

        public FileWatcher() {
            watcherThread = new Thread(Tick);
            watcherRunning = true;
            resetEvent.Set();
            watcherThread.Start();
        }

        public void SetFilesToWatch(List<GenericFile> files) {
            watcherRunning = false;
            resetEvent.Reset();
            watcherThread?.Join();

            fileExistsBuffer.Clear();
            filesToWatch.Clear();
            foreach (var item in files) {
                filesToWatch.Add(item);
            }

            watcherThread = new Thread(Tick);
            watcherRunning = true;
            resetEvent.Set();
            watcherThread.Start();
        }

        private void Tick() {
            while (watcherRunning && !globalStopRequested) {
                bool doUpdate = false;

                filesToWatch.ForEach(item => {
                    string identifier = item.Path + (item.Path.EndsWith("\\") ? "" : "\\") + item.TargetName;
                    if (!fileExistsBuffer.ContainsKey(identifier)) {
                        fileExistsBuffer.Add(identifier, File.Exists(identifier));
                    }

                    if (fileExistsBuffer[identifier] != File.Exists(identifier)) {
                        fileExistsBuffer[identifier] = File.Exists(identifier);
                        Debug.WriteLine($">>>>>> {identifier} changed");
                        doUpdate = true;
                    }
                });

                if (doUpdate) {
                    Thread t = new Thread(() => {
                        FileChangedOnFSEvent?.Invoke(this, EventArgs.Empty);
                    });
                    t.Start();
                }

                if (!resetEvent.WaitOne(1000)) {
                    break; // Exit the loop if the event is signaled
                }

                Thread.Sleep(1000);
            }
        }

        public void Stop() {
            watcherRunning = false;
            resetEvent.Reset();
            watcherThread?.Join();
            watcherThread = null;
        }

        public static void GlobalStop() {
            globalStopRequested = true;
        }
    }
}
