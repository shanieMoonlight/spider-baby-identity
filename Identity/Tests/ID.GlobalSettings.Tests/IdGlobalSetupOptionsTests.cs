using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;
using ID.GlobalSettings.Setup.Options;
using ID.GlobalSettings.Setup;

namespace ID.Domain.Tests.Setup.Settings;

public class IdGlobalSetupOptionsTests
{
    [Fact]
    public void AllProperties_ShouldHaveRequiredModifier()
    {
        // Arrange
        var type = typeof(IdGlobalOptions);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Act & Assert
        foreach (var property in properties)
        {
            var hasRequiredModifier = property.GetCustomAttributes()
                .Any(attr => attr.GetType().Name == "RequiredMemberAttribute");

            Assert.True(hasRequiredModifier,
                $"Property '{property.Name}' should have the 'required' modifier");
        }
    }

    //------------------------------------//
    //[Fact]
    //public void AllProperties_ShouldHaveRequiredModifier_CUSTOMER()
    //{
    //    // Arrange
    //    var type = typeof(Id_CUSTOMER_GlobalSetupOptions);
    //    var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //    // Act & Assert
    //    foreach (var property in properties)
    //    {
    //        var hasRequiredModifier = property.GetCustomAttributes()
    //            .Any(attr => attr.GetType().Name == "RequiredMemberAttribute");

    //        Assert.True(hasRequiredModifier,
    //            $"Property '{property.Name}' should have the 'required' modifier");
    //    }
    //}

    //------------------------------------//
}