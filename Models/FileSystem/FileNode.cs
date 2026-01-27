namespace Models.FileSystem;

public record FileNode(
    string Name,
    DateTime Created,
    DateTime Modified,
    string Extension = "")
{
    public string FullName => string.IsNullOrEmpty(Extension) ? Name : $"{Name}.{Extension}";
}
