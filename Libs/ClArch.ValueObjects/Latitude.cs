using ClArch.ValueObjects.Common;

namespace ClArch.ValueObjects;
public class Latitude : DoubleValueObject
{
    private Latitude(double value) : base(value) { }

    public static Latitude Create(double value) =>
        new(value);
}
