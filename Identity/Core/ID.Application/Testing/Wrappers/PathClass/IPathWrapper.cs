namespace ID.GlobalSettings.Testing.Wrappers.PathClass;
public interface IPathWrapper
{
    string Combine(params string[] paths);
    string GetFileName(string path);
    bool Exists(string path);
    string? GetDirectoryName(string path);
}
