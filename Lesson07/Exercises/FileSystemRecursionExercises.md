# File System Recursion Exercises

These exercises build upon the `FileSystem.cs` example to help you understand recursion through practical file system operations. Each exercise increases in difficulty and explores different aspects of recursive algorithms.

## File Structure

- **FileSystemExercises.cs**: Contains the exercise templates with placeholders for you to implement
- **FileSystemExercisesAnswers.cs**: Contains the complete solutions to all exercises
- **Models**: Extended models with `FileNodeExtended` and `DirectoryNodeExtended` that include `Size` property

## Understanding the Base Code

The file system implementation uses a tree structure where:
- **DirectoryNodeExtended**: Represents a directory with files and subdirectories (extends the base model with Size support)
- **FileNodeExtended**: Represents a single file with size information (extends the base model)
- **FileSystemTreeExtended**: Provides operations on the entire tree structure
- **FileSystemBuilderWithSize**: Builder that scans the file system and includes file sizes

Key recursive methods already implemented:
- `ScanDirectory()`: Recursively scans directories to build the tree
- `GetAllFilesRecursively()`: Collects all files from a directory and its subdirectories (needs implementation in exercises)

---

## Exercise 1: Calculate Total File Size

**Objective**: Implement a method to calculate the total size of all files in the file system tree.

### Task
Implement the following method in `DirectoryNodeExtended`:
```csharp
public long GetTotalSizeInBytes()
{
    // Base case: sum of all files in current directory
    
    // Recursive case: add sizes from all subdirectories
    
    return 0; // Placeholder
}
```

### Requirements
- Recursively traverse all subdirectories
- For each directory, sum the sizes of all files in that directory
- Add the sizes from all subdirectories
- Return the total size in bytes

### Hints
- Base case: Sum of all file sizes in the current directory (use LINQ's `Sum()`)
- Recursive case: Add the recursive call results from all subdirectories
- The `FileNodeExtended` already has a `Size` property
- Formula: `currentDirSize + subdirectoriesSize`

### Solution Approach
```csharp
var currentDirSize = ...
var subdirectoriesSize = ...
return currentDirSize + subdirectoriesSize;
```

### Expected Output
```
Total size: 15,234,567 bytes (14.53 MB)
```

---

## Exercise 2: Find Files by Extension

**Objective**: Implement a method to search for all files with a specific extension recursively.

### Task
Implement the following method in `DirectoryNodeExtended`:
```csharp
public ImmutableList<FileNodeExtended> FindFilesByExtension(string extension)
{
    // Normalize extension (remove dot, make lowercase)
    
    // Base case: filter files in current directory
    
    // Recursive case: add matching files from all subdirectories
    
    return ImmutableList<FileNodeExtended>.Empty; // Placeholder
}
```

### Requirements
- Search the current directory and all subdirectories recursively
- Return all files that match the given extension (case-insensitive)
- Empty directories should return an empty list
- Handle the case where extension is passed with or without a dot (e.g., ".cs" or "cs")

### Hints
- Normalize the extension: `extension.TrimStart('.').ToLower()`
- Base case: Filter files in current directory using LINQ's `Where()`
- Recursive case: Iterate through subdirectories and merge results
- Use `AddRange()` to combine results from subdirectories

### Solution Approach
```csharp
var normalizedExt = ...;
var matchingFiles = ...
foreach (var subdir in Subdirectories)
{
    ...
}
return matchingFiles.ToImmutableList();
```

### Expected Output
```
Found 23 .cs files:
  - Program.cs
  - FileSystem.cs
  - DirectoryNode.cs
  ...
```

---

## Exercise 3: Calculate Maximum Depth 

**Objective**: Determine the maximum depth of the directory tree.

### Task
Implement the following method in `DirectoryNodeExtended`:
```csharp
public int GetMaxDepth()
{
    // Base case: no subdirectories means depth is 0
    
    // Recursive case: 1 + maximum depth of all subdirectories
    
    return 0; // Placeholder
}
```

And in `FileSystemTreeExtended`:
```csharp
public int GetMaxDepth() => Root.GetMaxDepth();
```

### Requirements
- The root directory starts at depth 0
- Each subdirectory level adds 1 to the depth
- Return the maximum depth found in any branch of the tree
- An empty directory (no subdirectories) has depth 0

### Hints
- Base case: Check if `Subdirectories.IsEmpty`, return 0
- Recursive case: Use LINQ's `Max()` on all subdirectory depths
- Formula: `1 + Subdirectories.Max(subdir => subdir.GetMaxDepth())`
- Handle empty subdirectories before calling `Max()`

### Solution Approach
```csharp
if (Subdirectories.IsEmpty)
    return 0;
return 1 + ...
```

### Expected Output
```
Maximum directory depth: 5 levels
```

---

## Exercise 4: Find Largest File Path 

**Objective**: Find the path to the largest file in the entire file system tree.

### Task
Implement the following method in `DirectoryNodeExtended`:
```csharp
public (FileNodeExtended? LargestFile, string Path) FindLargestFile(string currentPath = "")
{
    // Initialize current path
    var thisPath = string.IsNullOrEmpty(currentPath) ? Name : $"{currentPath}/{Name}";
    
    // Find largest file in current directory
    FileNodeExtended? largestInCurrent = null;
    string largestPath = "";
    
    // Recursively check all subdirectories
    foreach (var subdir in Subdirectories)
    {
        // Compare with results from subdirectories
    }
    
    return (largestInCurrent, largestPath);
}
```

### Requirements
- Recursively search all directories for the largest file
- Return both the file and its full path
- If multiple files have the same size, return any one
- Handle empty directories (no files at all) by returning `null` for the file
- The path should be in the format: `/root/subdirectory/filename.ext`

### Hints
- Build the current path as you recurse: `currentPath + "/" + Name`
- Find largest in current directory: `Files.OrderByDescending(f => f.Size).FirstOrDefault()`
- Recursively call on each subdirectory with updated path
- Compare sizes and keep track of the largest found
- Handle null checks when comparing files

### Solution Approach
```csharp
if (Files.Any())
{
    largestInCurrent = ...
    largestPath = ...;
}

foreach (var subdir in Subdirectories)
{
...
}
```

### Expected Output
```
Largest file: video.mp4
Path: /Users/Martin/Documents/Videos/video.mp4
Size: 523,456,789 bytes (499.15 MB)
```

---

## Exercise 5: Generate File System Report with Statistics (Very Hard)

**Objective**: Create a comprehensive report that analyzes file types, sizes, and dates using multiple recursive operations.

### Task
Implement multiple helper methods in `DirectoryNodeExtended`:

```csharp
public Dictionary<string, int> GetFilesByExtensionRecursive()
public Dictionary<string, long> GetSizeByExtensionRecursive()
public (FileNodeExtended? OldestFile, DateTime OldestDate) GetOldestFileRecursive()
public (FileNodeExtended? NewestFile, DateTime NewestDate) GetNewestFileRecursive()
public (DirectoryNodeExtended? DeepestDir, int Depth, string Path) GetDeepestDirectoryRecursive(string currentPath = "", int currentDepth = 0)
public ImmutableList<FileNodeExtended> GetAllFilesRecursively()
public ImmutableList<DirectoryNodeExtended> GetAllDirectoriesRecursively()
```

Then implement in `FileSystemTreeExtended`:
```csharp
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
    
    return new FileSystemReport(...);
}
```

The report record is already defined:
```csharp
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
```

### Requirements
- Calculate total directories and files recursively
- Calculate total size (reuse Exercise 1)
- Find the maximum depth (reuse Exercise 3)
- Group files by extension and count them recursively
- Group files by extension and sum their sizes recursively
- Find the oldest file (by creation date) recursively
- Find the newest file (by modification date) recursively
- Find the deepest directory and its path recursively

### Hints for Each Helper Method

**GetFilesByExtensionRecursive():**
- Create a dictionary to accumulate results
- Add files from current directory to the dictionary
- Recursively get dictionaries from subdirectories
- Merge subdirectory results using `GetValueOrDefault()`

**GetSizeByExtensionRecursive():**
- Similar to files by extension, but sum sizes instead of counts
- Accumulate `file.Size` for each extension

**GetOldestFileRecursive():**
- Track the oldest file and its creation date
- Initialize with `DateTime.MaxValue`
- Compare with files in current directory
- Recursively check subdirectories and update if older found

**GetNewestFileRecursive():**
- Similar to oldest, but use `DateTime.MinValue` and find most recent

**GetDeepestDirectoryRecursive():**
- Pass current depth as parameter
- Track deepest directory found and its path
- Recursively check subdirectories with `depth + 1`

**GetAllFilesRecursively() / GetAllDirectoriesRecursively():**
- Start with items from current level
- Recursively add from all subdirectories using `AddRange()`

### Expected Output
```
=== File System Report ===

Structure:
  Total Directories: 127
  Total Files: 1,453
  Maximum Depth: 6 levels

Size Analysis:
  Total Size: 2,567,890,123 bytes (2.39 GB)
  
  By Extension:
    .cs:   523 files (15.2 MB)
    .md:   89 files (1.3 MB)
    .json: 45 files (234 KB)
    .png:  156 files (45.6 MB)
    .mp4:  12 files (1.8 GB)
    (other): 628 files (523.4 MB)

File Age:
  Oldest File: legacy.txt (Created: 2015-03-15)
  Newest File: Report.pdf (Modified: 2026-01-27)

Directory Structure:
  Deepest Directory: /Users/Martin/Documents/Projects/Archive/Old/Backup/Legacy
  Depth: 6 levels
```

---

## Learning Objectives Summary

After completing these exercises, you should understand:

1. **Basic Recursion** (Ex 1-2): How to traverse tree structures recursively and accumulate results
2. **Depth Calculation** (Ex 3): How to use recursion to measure tree properties
3. **Path Tracking** (Ex 4): How to maintain context (like paths) during recursive traversal
4. **Complex Aggregation** (Ex 5): How to combine multiple recursive operations efficiently

### Key Recursive Patterns Used

- **Accumulator Pattern**: Building up a result as you recurse (Ex 1, 2)
- **Maximum/Minimum Pattern**: Finding extremes through recursion (Ex 3, 4)
- **Path Building Pattern**: Constructing paths during traversal (Ex 4)
- **Multi-Aggregation Pattern**: Collecting multiple statistics in one pass (Ex 5)
- **Dictionary Merging Pattern**: Combining results from multiple branches (Ex 5)

### Important Recursive Concepts

1. **Base Case**: The condition that stops recursion (e.g., empty subdirectories, no files)
2. **Recursive Case**: The part that calls itself on a smaller problem (e.g., calling method on subdirectories)
3. **Accumulation**: Building up results as you recurse through the tree
4. **Trust the Recursion**: Assume the recursive call correctly handles the subproblem

---

Good luck! Remember: the key to understanding recursion is trusting that the recursive call will handle the smaller subproblem correctly.

---

## How to Work with These Exercises

1. **Start with FileSystemExercises.cs** - This file contains method stubs with placeholders for you to implement
2. **Read each exercise description** in this markdown file carefully
3. **Implement the methods** one by one, following the hints provided
4. **Test your implementation** by running the `RunExamples()` method
5. **Compare with FileSystemExercisesAnswers.cs** if you get stuck or want to verify your solution

### Running the Exercises

In your `Main` program or lesson runner:
```csharp
using Playground.Lesson07.Exercises;

FileSystemExercises.RunExamples();
```

To see the complete solutions:
```csharp
using Playground.Lesson07.ExerciseAnswers;

FileSystemExercises.RunExamples(); // Note: same class name but different namespace
```

---

## Testing Your Solutions

The exercises use a real directory path:
```csharp
var startingPath = "/Users/Martin/Development/goldenProjects/FunctionalProgramming";
```

Update this path to point to a directory on your system. The builder automatically skips common build directories (`node_modules`, `bin`, `obj`, `.git`) to keep the results manageable.

You can also create a test directory structure:
```csharp
var testPath = Path.Combine(Path.GetTempPath(), "RecursionTest");
Directory.CreateDirectory(testPath);
// Create subdirectories and files for testing
var tree = FileSystemBuilderWithSize.Build(testPath);
```

---

## Learning Objectives Summary
