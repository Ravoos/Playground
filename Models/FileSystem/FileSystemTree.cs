using System.Text;

namespace Models.FileSystem;

public record FileSystemTree (DirectoryNode Root)
{
    public FileSystemStats GetStatistics()
    {
        var allFiles = Root.GetAllFilesRecursively();
        var allDirs = Root.GetAllDirectoriesRecursively();
        
        return new FileSystemStats(
            TotalDirectories: allDirs.Count + 1, // +1 for root
            TotalFiles: allFiles.Count);
    }

    public string GenerateTreeView(bool showFiles = true)
    {
        var sb = new StringBuilder();
        GenerateTreeViewRecursive(Root, sb, "", true, showFiles);
        return sb.ToString();
    }

    private static void GenerateTreeViewRecursive(DirectoryNode dir, StringBuilder sb, string indent, bool isLast, bool showFiles)
    {
        sb.AppendLine($"{indent}{(isLast ? "└── " : "├── ")}{dir.Name}/");
        
        var newIndent = indent + (isLast ? "    " : "│   ");
        
        // Show files first
        if (showFiles)
        {
            var files = dir.Files;
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var isLastFile = i == files.Count - 1 && dir.Subdirectories.IsEmpty;
                sb.AppendLine($"{newIndent}{(isLastFile ? "└── " : "├── ")}{file.FullName}");
            }
        }
        
        // Show subdirectories
        for (int i = 0; i < dir.Subdirectories.Count; i++)
        {
            var subdir = dir.Subdirectories[i];
            var isLastSubdir = i == dir.Subdirectories.Count - 1;
            GenerateTreeViewRecursive(subdir, sb, newIndent, isLastSubdir, showFiles);
        }
    }
}