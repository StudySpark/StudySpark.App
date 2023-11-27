namespace StudySpark.Core.FileManager;
public class SearchFiles {
    public List<string> GetFilesFromDir(string path, string extension, SearchOption searchOption)
    {
        List<string> sortedFilesByExtension = new();

        var sortedFiles = new DirectoryInfo(path).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
        foreach (var file in sortedFiles)
        {
            if (file.ToString().ToLower().Contains(extension.ToLower()))
            {
                sortedFilesByExtension.Add(file.ToString());
            }
        }
        return sortedFilesByExtension;
    }
}