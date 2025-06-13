using ClArch.ValueObjects.Common;

namespace ClArch.ValueObjects;


public class EmailVerifiedNullable : ValueObject<bool?>
{
    private EmailVerifiedNullable(bool? value) : base(value) { }

    public static EmailVerifiedNullable Create(bool? value) => new(value);

}//Cls


