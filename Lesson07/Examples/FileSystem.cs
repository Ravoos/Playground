using System.Collections.Immutable;
using Models.FileSystem;

namespace Playground.Lesson07;

public static class FileSystem
{
    public static void RunExamples()
    {
        var startingPath = "/Users/Martin/Development/projects/Advanced_Programming_Dec_2025"; 
        var fileSystemTree = FileSystemBuilder.Build(startingPath);

        var stats = fileSystemTree.GetStatistics();
        Console.WriteLine($"Total Directories: {stats.TotalDirectories}");
        Console.WriteLine($"Total Files: {stats.TotalFiles}");
        Console.WriteLine();

        var treeView = fileSystemTree.GenerateTreeView(showFiles: false);
        Console.WriteLine("File System Tree View:");
        Console.WriteLine(treeView);
    }

    public class FileSystemBuilder
    {
        private DirectoryNode _root = null;

        public static FileSystemTree Build(string startingPath)
        {
            if (!Directory.Exists(startingPath))
                throw new DirectoryNotFoundException($"Directory not found: {startingPath}");

            var builder = new FileSystemBuilder();
            var rootInfo = new DirectoryInfo(startingPath);
            
            //Recuresively scan the directory structure
            builder._root = builder.ScanDirectory(rootInfo);
            
            return new FileSystemTree(builder._root);
        }

        private DirectoryNode ScanDirectory(DirectoryInfo directoryInfo)
        {
            var files = ImmutableList<FileNode>.Empty;
            var subdirectories = ImmutableList<DirectoryNode>.Empty;

            try
            {
                // Scan files in the directory
                foreach (var fileInfo in directoryInfo.GetFiles())
                {
                    var extension = string.IsNullOrEmpty(fileInfo.Extension) ? "" : fileInfo.Extension.TrimStart('.');
                    var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    
                    if (string.IsNullOrEmpty(fileName))
                        fileName = fileInfo.Name; // Handle files that start with dot or have no extension
                    
                    var fileNode = new FileNode(fileName, fileInfo.CreationTime, fileInfo.LastWriteTime, extension);
                    files = files.Add(fileNode);
                }

                // Scan subdirectories recursively
                foreach (var subDirInfo in directoryInfo.GetDirectories())
                {
                    // Skip hidden directories and system directories
                    if ((subDirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                        (subDirInfo.Attributes & FileAttributes.System) == FileAttributes.System)
                        continue;

                    var subDirectoryNode = ScanDirectory(subDirInfo);
                    subdirectories = subdirectories.Add(subDirectoryNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories we don't have access to
                // Could log this if needed
            }
            catch (IOException)
            {
                // Skip directories with IO issues
                // Could log this if needed
            }

            return new DirectoryNode(
                directoryInfo.Name, 
                directoryInfo.CreationTime, 
                directoryInfo.LastWriteTime,
                files,
                subdirectories);
        }
    }
}
