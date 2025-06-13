using ClArch.ValueObjects.Common;
using ClArch.ValueObjects.Setup;
using ClArch.ValueObjects.Utility;

namespace ClArch.ValueObjects;
public class FileNameWithoutExtension : StringInvariantValueObject
{
    public readonly static int MaxLength = ValueObjectsSettings.MaxLengthFileName;


    private FileNameWithoutExtension(string value) : base(value) { }



    public static FileNameWithoutExtension Create(string value)
    {
        Ensure.MaxLengthTrimmed(value,MaxLength, nameof(FileNameWithoutExtension));
        return new(value);
    }

    
    public static FileNameWithoutExtension Create(Guid value)
    {
        return new(value.ToString());
    }

}//Cls