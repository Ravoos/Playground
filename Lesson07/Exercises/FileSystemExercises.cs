using System.Collections.Immutable;
using System.Text;
using Models.FileSystem;

namespace Playground.Lesson07.Exercises;

public static class FileSystemExercises
{
    public static void RunExamples()
    {
        var startingPath = "/Users/Martin/Development/goldenProjects/FunctionalProgramming";
        
        if (!Directory.Exists(startingPath))
        {
            Console.WriteLine($"Path not found: {startingPath}");
            Console.WriteLine("Please update the path in the code.");
            return;
        }

        var fileSystemTree = FileSystemBuilderWithSize.Build(startingPath);

        Console.WriteLine("\n=== Exercise 1: Total File Size ===");
        RunExercise1(fileSystemTree);

        Console.WriteLine("\n=== Exercise 2: Find Files by Extension ===");
        RunExercise2(fileSystemTree);

        Console.WriteLine("\n=== Exercise 3: Calculate Maximum Depth ===");
        RunExercise3(fileSystemTree);

        Console.WriteLine("\n=== Exercise 4: Find Largest File Path ===");
        RunExercise4(fileSystemTree);

        Console.WriteLine("\n=== Exercise 5: Generate Detailed Report ===");
        RunExercise5(fileSystemTree);
    }

    private static void RunExercise1(FileSystemTreeExtended tree)
    {
        var totalSize = tree.Root.GetTotalSizeInBytes();
        var sizeInMB = totalSize / (1024.0 * 1024.0);
        var sizeInGB = totalSize / (1024.0 * 1024.0 * 1024.0);

        Console.WriteLine($"Total size: {totalSize:N0} bytes");
        if (sizeInGB >= 1)
            Console.WriteLine($"  ({sizeInGB:F2} GB)");
        else
            Console.WriteLine($"  ({sizeInMB:F2} MB)");
    }

    private static void RunExercise2(FileSystemTreeExtended tree)
    {
        var extensions = new[] { "cs", "md", "json" };
        
        foreach (var ext in extensions)
        {
            var files = tree.Root.FindFilesByExtension(ext);
            Console.WriteLine($"\nFound {files.Count} .{ext} files:");
            
            foreach (var file in files.Take(5))
            {
                Console.WriteLine($"  - {file.FullName}");
            }
            
            if (files.Count > 5)
                Console.WriteLine($"  ... and {files.Count - 5} more");
        }
    }

    private static void RunExercise3(FileSystemTreeExtended tree)
    {
        var maxDepth = tree.GetMaxDepth();
        Console.WriteLine($"Maximum directory depth: {maxDepth} levels");
    }

    private static void RunExercise4(FileSystemTreeExtended tree)
    {
        var (largestFile, path) = tree.Root.FindLargestFile();
        
        if (largestFile != null)
        {
            var sizeInMB = largestFile.Size / (1024.0 * 1024.0);
            Console.WriteLine($"Largest file: {largestFile.FullName}");
            Console.WriteLine($"Path: {path}");
            Console.WriteLine($"Size: {largestFile.Size:N0} bytes ({sizeInMB:F2} MB)");
        }
        else
        {
            Console.WriteLine("No files found in the tree.");
        }
    }

    private static void RunExercise5(FileSystemTreeExtended tree)
    {
        var report = tree.GenerateDetailedReport();
        
        Console.WriteLine("=== File System Report ===\n");
        
        Console.WriteLine("Structure:");
        Console.WriteLine($"  Total Directories: {report.TotalDirectories}");
        Console.WriteLine($"  Total Files: {report.TotalFiles}");
        Console.WriteLine($"  Maximum Depth: {report.MaxDepth} levels");
        
        Console.WriteLine("\nSize Analysis:");
        var totalGB = report.TotalSize / (1024.0 * 1024.0 * 1024.0);
        var totalMB = report.TotalSize / (1024.0 * 1024.0);
        Console.WriteLine($"  Total Size: {report.TotalSize:N0} bytes ({(totalGB >= 1 ? $"{totalGB:F2} GB" : $"{totalMB:F2} MB")})");
        
        // Uncomment below to show detailed extension report
        // Commented out to avoid null exceptions in case methods are not fully implemented
        
        // Console.WriteLine("\n  By Extension:");
        // var topExtensions = report.FilesByExtension
        //     .OrderByDescending(kvp => kvp.Value)
        //     .Take(10);
        
        // foreach (var (ext, count) in topExtensions)
        // {
        //     var size = report.SizeByExtension.GetValueOrDefault(ext, 0);
        //     var sizeMB = size / (1024.0 * 1024.0);
        //     var extDisplay = string.IsNullOrEmpty(ext) ? "(no extension)" : $".{ext}";
        //     Console.WriteLine($"    {extDisplay,-15} {count,4} files ({sizeMB,8:F2} MB)");
        // }
        
        // Console.WriteLine("\nFile Age:");
        // if (report.OldestFile != null)
        //     Console.WriteLine($"  Oldest File: {report.OldestFile.FullName} (Created: {report.OldestFile.Created:yyyy-MM-dd})");
        // if (report.NewestFile != null)
        //     Console.WriteLine($"  Newest File: {report.NewestFile.FullName} (Modified: {report.NewestFile.Modified:yyyy-MM-dd})");
        
        // if (report.DeepestDirectory != null)
        // {
        //     Console.WriteLine("\nDirectory Structure:");
        //     Console.WriteLine($"  Deepest Directory: {report.DeepestDirectoryPath}");
        //     Console.WriteLine($"  Depth: {report.MaxDepth} levels");
        // }
    }
}

// Extended models with Size support for Exercise 1

public record FileNodeExtended(
    string Name,
    DateTime Created,
    DateTime Modified,
    string Extension,
    long Size) : FileNode(Name, Created, Modified, Extension);

public record DirectoryNodeExtended(
    string Name,
    DateTime Created,
    DateTime Modified,
    ImmutableList<FileNodeExtended> Files,
    ImmutableList<DirectoryNodeExtended> Subdirectories)
{
    // Exercise 1: Calculate Total File Size
    public long GetTotalSizeInBytes()
    {
        // Base case: sum of all files in current directory
        
        // Recursive case: add sizes from all subdirectories
        
        return 0; // Placeholder
    }

    // Exercise 2: Find Files by Extension
    public ImmutableList<FileNodeExtended> FindFilesByExtension(string extension)
    {
        // Normalize extension (remove dot, make lowercase)
        
        // Base case: filter files in current directory
        
        // Recursive case: add matching files from all subdirectories
        
        return ImmutableList<FileNodeExtended>.Empty; // Placeholder
    }

    // Exercise 3: Calculate Maximum Depth
    public int GetMaxDepth()
    {
        // Base case: no subdirectories means depth is 0
        
        // Recursive case: 1 + maximum depth of all subdirectories
        return 0;
    }

    // Exercise 4: Find Largest File Path
    public (FileNodeExtended? LargestFile, string Path) FindLargestFile(string currentPath = "")
    {
        // Initialize current path
        var thisPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath}/{Name}";
        
        // Find largest file in current directory
        FileNodeExtended? largestInCurrent = null;
        string largestPath = "";
                
        // Recursive case: check all subdirectories
        foreach (var subdir in Subdirectories)
        {
        }
        
        // base case: return largest found
        return (largestInCurrent, largestPath);
    }

    // Exercise 5 Helper: Get files grouped by extension with counts and sizes
    public Dictionary<string, int> GetFilesByExtensionRecursive()
    {
        var result = new Dictionary<string, int>();
        
        // Add files from current directory into dictionary
        foreach (var file in Files)
        {
            var ext = file.Extension.ToLower();
            result[ext] = result.GetValueOrDefault(ext, 0) + 1;
        }
        
        // Recursively add from subdirectories
        
        
        return null; // Placeholder
    }

    public Dictionary<string, long> GetSizeByExtensionRecursive()
    {
        var result = new Dictionary<string, long>();
        
        // Add sizes from current directory
        
        // Recursively add from subdirectories

        return null; // Placeholder
    }

    public (FileNodeExtended? OldestFile, DateTime OldestDate) GetOldestFileRecursive()
    {
        FileNodeExtended? oldest = null;
        DateTime oldestDate = DateTime.MaxValue;
        
        // Check files in current directory
        
        // Recursively check subdirectories
        
        return (oldest, oldestDate);
    }

    public (FileNodeExtended? NewestFile, DateTime NewestDate) GetNewestFileRecursive()
    {
        FileNodeExtended? newest = null;
        DateTime newestDate = DateTime.MinValue;
        
        // Check files in current directory
        
        // Recursively check subdirectories

        return (newest, newestDate);
    }

    public (DirectoryNodeExtended? DeepestDir, int Depth, string Path) GetDeepestDirectoryRecursive(string currentPath = "", int currentDepth = 0)
    {
        var thisPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath}/{Name}";
        
        // Start with current directory
        DirectoryNodeExtended? deepestDir = this;
        int maxDepth = currentDepth;
        string deepestPath = thisPath;
        
        // Recursively check all subdirectories

        
        return (deepestDir, maxDepth, deepestPath);
    }

    public ImmutableList<FileNodeExtended> GetAllFilesRecursively()
    {
        var allFiles = Files.ToList();

        // Recursively add files from subdirectories

        return allFiles.ToImmutableList();
    }

    public ImmutableList<DirectoryNodeExtended> GetAllDirectoriesRecursively()
    {
        var allDirs = Subdirectories.ToList();

        // Recursively add directories from subdirectories

        return allDirs.ToImmutableList();
    }
}

// Extended FileSystemTree with additional methods
public record FileSystemTreeExtended(DirectoryNodeExtended Root)
{
    // Exercise 3: Get maximum depth
    public int GetMaxDepth() => Root.GetMaxDepth();

    // Exercise 5: Generate comprehensive report
    public FileSystemReport GenerateDetailedReport()
    {
        var allFiles = Root.GetAllFilesRecursively();
        var allDirs = Root.GetAllDirectoriesRecursively();
        
        var totalSize = Root.GetTotalSizeInBytes();
        var maxDepth = GetMaxDepth();
        var filesByExtension = Root.GetFilesByExtensionRecursive();
        var sizeByExtension = Root.GetSizeByExtensionRecursive();
        
        var (oldestFile, _) = Root.GetOldestFileRecursive();
        var (newestFile, _) = Root.GetNewestFileRecursive();
        var (deepestDir, _, deepestPath) = Root.GetDeepestDirectoryRecursive();
        
        return new FileSystemReport(
            TotalDirectories: allDirs.Count + 1, // +1 for root
            TotalFiles: allFiles.Count,
            TotalSize: totalSize,
            MaxDepth: maxDepth,
            FilesByExtension: filesByExtension,
            SizeByExtension: sizeByExtension,
            OldestFile: oldestFile,
            NewestFile: newestFile,
            DeepestDirectory: deepestDir,
            DeepestDirectoryPath: deepestPath
        );
    }
}

// Exercise 5: Report record
public record FileSystemReport(
    int TotalDirectories,
    int TotalFiles,
    long TotalSize,
    int MaxDepth,
    Dictionary<string, int> FilesByExtension,
    Dictionary<string, long> SizeByExtension,
    FileNodeExtended? OldestFile,
    FileNodeExtended? NewestFile,
    DirectoryNodeExtended? DeepestDirectory,
    string DeepestDirectoryPath
);

// Builder that includes file sizes
public class FileSystemBuilderWithSize
{
    private DirectoryNodeExtended? _root = null;

    public static FileSystemTreeExtended Build(string startingPath)
    {
        if (!Directory.Exists(startingPath))
            throw new DirectoryNotFoundException($"Directory not found: {startingPath}");

        var builder = new FileSystemBuilderWithSize();
        var rootInfo = new DirectoryInfo(startingPath);
        
        builder._root = builder.ScanDirectory(rootInfo);
        
        return new FileSystemTreeExtended(builder._root);
    }

    private DirectoryNodeExtended ScanDirectory(DirectoryInfo directoryInfo)
    {
        var files = ImmutableList<FileNodeExtended>.Empty;
        var subdirectories = ImmutableList<DirectoryNodeExtended>.Empty;

        try
        {
            // Scan files in the directory
            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                var extension = string.IsNullOrEmpty(fileInfo.Extension) ? "" : fileInfo.Extension.TrimStart('.');
                var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                
                if (string.IsNullOrEmpty(fileName))
                    fileName = fileInfo.Name;
                
                var fileNode = new FileNodeExtended(
                    fileName, 
                    fileInfo.CreationTime, 
                    fileInfo.LastWriteTime, 
                    extension,
                    fileInfo.Length); // Include file size
                    
                files = files.Add(fileNode);
            }

            // Scan subdirectories recursively
            foreach (var subDirInfo in directoryInfo.GetDirectories())
            {
                // Skip hidden, system directories, and common ignored directories
                if ((subDirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                    (subDirInfo.Attributes & FileAttributes.System) == FileAttributes.System ||
                    subDirInfo.Name == "node_modules" ||
                    subDirInfo.Name == "bin" ||
                    subDirInfo.Name == "obj" ||
                    subDirInfo.Name == ".git")
                    continue;

                var subDirectoryNode = ScanDirectory(subDirInfo);
                subdirectories = subdirectories.Add(subDirectoryNode);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have access to
        }
        catch (IOException)
        {
            // Skip directories with IO issues
        }

        return new DirectoryNodeExtended(
            directoryInfo.Name, 
            directoryInfo.CreationTime, 
            directoryInfo.LastWriteTime,
            files,
            subdirectories);
    }
}
