namespace ID.GlobalSettings.Testing.Wrappers.PathClass;
internal class PathWrapper : IPathWrapper
{
    public string Combine(string[] paths) =>
        Path.Combine(paths);


    public bool Exists(string path) =>
        Path.Exists(path);


    public string? GetDirectoryName(string path) =>
        Path.GetDirectoryName(path);


    public string GetFileName(string path) =>
        Path.GetFileName(path);


}
