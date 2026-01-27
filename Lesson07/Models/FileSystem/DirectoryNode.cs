using System.Collections.Immutable;

namespace Models.FileSystem;

public record DirectoryNode(
    string Name,
    DateTime Created,
    DateTime Modified,
    ImmutableList<FileNode> Files,
    ImmutableList<DirectoryNode> Subdirectories)
{
    public DirectoryNode(string name, DateTime created, DateTime modified) : this(name, created, modified, ImmutableList<FileNode>.Empty, ImmutableList<DirectoryNode>.Empty) {}

    public bool IsEmpty => Files.IsEmpty && Subdirectories.IsEmpty;

    public ImmutableList<FileNode> GetAllFilesRecursively()
    {
        var allFiles = Files.ToList();
        foreach (var subdir in Subdirectories)
        {
            allFiles.AddRange(subdir.GetAllFilesRecursively());
        }
        return allFiles.ToImmutableList();
    }

    public ImmutableList<DirectoryNode> GetAllDirectoriesRecursively()
    {
        var allDirs = Subdirectories.ToList();
        foreach (var subdir in Subdirectories)
        {
            allDirs.AddRange(subdir.GetAllDirectoriesRecursively());
        }
        return allDirs.ToImmutableList();
    }
}

