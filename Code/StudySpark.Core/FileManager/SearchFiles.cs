namespace StudySpark.Core.FileManager;
public class SearchFiles {
    //public static List<string> GetFilesFromRecent(string extension, SearchOption searchOption)
    //{
    //    string path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
    //    List<string> files = Directory.EnumerateFiles(path, "*" + extension, searchOption).ToList();



    //    return files;
    //}

    public static List<string> GetFilesFromRecent(string extension, SearchOption searchOption)
    {
        List<string> sortedSLNFiles = new();
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
        //List<string> files = Directory.EnumerateFiles(path, "*" + extension, searchOption).ToList();

        var sortedFiles = new DirectoryInfo(path).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
        foreach (var file in sortedFiles)
        {
            if (file.ToString().ToLower().Contains(extension.ToLower()))
            {
                sortedSLNFiles.Add(file.ToString());
            }
        }

        return sortedSLNFiles;
    }
    public static List<string> GetLastDownloadedFiles(SearchOption searchOption)
    {
        List<string> sortedDownloadFiles = new();
        // Implement logic to get the last downloaded files
        // For example, you can use DirectoryInfo to get files from a specific directory
        string downloadsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        var sortedFiles = new DirectoryInfo(downloadsPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();

        // Get the last 'count' files ordered by creation time
        foreach (var file in sortedFiles)
        {
            file.ToString().ToLower();
            sortedDownloadFiles.Add(file.ToString());
        }
    return sortedDownloadFiles;
    }
}

