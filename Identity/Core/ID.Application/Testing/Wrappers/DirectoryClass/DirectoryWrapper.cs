namespace ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
internal class DirectoryWrapper : IDirectoryWrapper
{

    public bool Exists(string path) =>
        Directory.Exists(path);


    public DirectoryInfo CreateDirectory(string path) =>
        Directory.CreateDirectory(path);



}
