namespace ID.GlobalSettings.Testing.Wrappers.DirectoryClass;
public interface IDirectoryWrapper
{
    DirectoryInfo CreateDirectory(string path);
    bool Exists(string path);
}
