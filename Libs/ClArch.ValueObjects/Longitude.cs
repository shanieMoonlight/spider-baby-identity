using ClArch.ValueObjects.Common;

namespace ClArch.ValueObjects;
public class Longitude : DoubleValueObject
{
    private Longitude(double value) : base(value) { }

    public static Longitude Create(double value) =>
        new(value);
}
