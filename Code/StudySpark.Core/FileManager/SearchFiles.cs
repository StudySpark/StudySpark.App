namespace StudySpark.Core.FileManager;
public class SearchFiles {
    public static List<string> GetFilesFromRecent(string extension, SearchOption searchOption)
    {
        List<string> sortedSLNFiles = new();
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Recent);

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
}