namespace ID.GlobalSettings.Testing.Wrappers.FileClass;
internal class FileWrapper : IFileWrapper
{

    public bool Exists(string path) =>
        File.Exists(path);



}
