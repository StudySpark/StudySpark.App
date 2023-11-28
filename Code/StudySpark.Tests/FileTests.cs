using StudySpark.Core.FileManager;

namespace StudySpark.Tests {
    [TestFixture]
    public class FileHelperTests {
        [Test]
        public void TestGetFilesFromDir() {
            SearchFiles searchFiles = new();
            // Arrange
            var dirPath = "testdir\\";
            var extension = ".sln.lnk";
            var searchOption = SearchOption.TopDirectoryOnly;
            var expectedFiles = new List<string>();

            if (Directory.Exists(dirPath)) {
                DirectoryInfo di = new DirectoryInfo(dirPath);
                foreach (FileInfo file in di.GetFiles()) {
                    file.Delete();
                }
                Directory.Delete(dirPath, true);
            }
            Directory.CreateDirectory(dirPath);

            for (int i = 0; i < 10; i++) {
                string fileToAdd = "test" + i + extension;
                File.Create(dirPath + fileToAdd);
                expectedFiles.Add(Path.GetFullPath(dirPath) + fileToAdd);
            }

            // Act
            var actualFiles = searchFiles.GetFilesFromDir(dirPath, extension, searchOption);

            // Assert
            Assert.That(actualFiles.Count, Is.EqualTo(expectedFiles.Count));

            // Check if all expected files are present in the actual result
            foreach (var actualFile in actualFiles) {
                Assert.Contains(actualFile, expectedFiles);
            }
        }

        [Test]
        public void TestGetFilesFromDirNoFiles() {
            SearchFiles searchFiles = new();
            // Arrange
            var dirPath = "testdir\\";
            var extension = ".sln.lnk";
            var searchOption = SearchOption.TopDirectoryOnly;
            var expectedFiles = new List<string>();

            if (Directory.Exists(dirPath)) {
                DirectoryInfo di = new DirectoryInfo(dirPath);

                // Wait for file handles to be released before deleting files
                foreach (var file in di.GetFiles()) {
                    file.Refresh();
                    while (IsFileLocked(file)) {
                        System.Threading.Thread.Sleep(100);
                        file.Refresh();
                    }
                    file.Delete();
                }

                // Wait for file handles to be released before deleting directory
                while (IsDirectoryLocked(di)) {
                    System.Threading.Thread.Sleep(100);
                    di.Refresh();
                }

                Directory.Delete(dirPath, true);
            }

            Directory.CreateDirectory(dirPath);

            // Act
            var actualFiles = searchFiles.GetFilesFromDir(dirPath, extension, searchOption);

            // Assert
            Assert.That(actualFiles.Count, Is.EqualTo(expectedFiles.Count));

            // Check if all expected files are present in the actual result
            foreach (var actualFile in actualFiles) {
                Assert.Contains(actualFile, expectedFiles);
            }
        }

        private static bool IsFileLocked(FileInfo file) {
            try {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None)) {
                    stream.Close();
                }
            } catch (IOException) {
                return true;
            }

            return false;
        }

        private static bool IsDirectoryLocked(DirectoryInfo directory) {
            try {
                var _ = directory.GetFiles();
            } catch (IOException) {
                return true;
            }

            return false;
        }
    }
}