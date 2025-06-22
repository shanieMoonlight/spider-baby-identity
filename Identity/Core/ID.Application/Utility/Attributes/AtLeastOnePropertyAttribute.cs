using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ID.Application.Utility.Attributes;

/// <summary>
/// Validation attribute that ensures at least one of the specified properties has a non-null, non-empty value.
/// </summary>
/// <param name="propertyList">List of property names to validate</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AtLeastOnePropertyAttribute(params string[] propertyList) : ValidationAttribute
{
    private string[] PropertyList { get; set; } = propertyList;


    //See http://stackoverflow.com/a/1365669
    public override object TypeId { get { return this; } }



    //------------------------------//


    // Override FormatErrorMessage to dynamically build the error message
    public override string FormatErrorMessage(string name) =>
        $"You must supply at least one of [{string.Join(", ", PropertyList)}]";



    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        foreach (string propertyName in PropertyList)
        {
            try
            {
                PropertyInfo? propertyInfo = value?.GetType()?.GetProperty(propertyName);
                var propertyValue = propertyInfo?.GetValue(value, null);

                if (propertyInfo is not null && propertyValue is not null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
                    return ValidationResult.Success;

            }
            catch (Exception)
            {
                return new ValidationResult(
                    $"Error accessing property '{propertyName}'.",
                    [propertyName]
                );
            }
        }

        // Use a descriptive key for the error
        return new ValidationResult(
            FormatErrorMessage(validationContext.DisplayName),
            ["AtLeastOneProperty"] // This will be the key in the errors dictionary
        );
    }


}//Cls
