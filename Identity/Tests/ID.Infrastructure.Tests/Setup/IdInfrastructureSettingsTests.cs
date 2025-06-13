//using ID.GlobalSettings.Utility;
//using ID.Infrastructure.Setup;
//using ID.Infrastructure.Setup.Passwords;
//using ID.Infrastructure.Setup.SignIn;
//using System.Reflection;

//namespace ID.Infrastructure.Tests.Setup;

//public class IdInfrastructureSettingsTests(IdInfrastructureSettingsFixture _fixture)
//    : IClassFixture<IdInfrastructureSettingsFixture>
//{
//    public void Setup_Should_Set_All_Fields_Correctly()
//    {
//        var setupOptions = new IdInfrastructureSetupOptions
//        {
//            PasswordOptions = new IdPasswordOptions { RequiredLength = 12, RequireDigit = true },
//            SignInOptions = new IdSignInOptions { RequireConfirmedEmail = true },
//            CookieLoginPath = "/custom-login",
//            CookieLogoutPath = "/custom-logout",
//            CookieAccessDeniedPath = "/custom-access-denied",
//            CookieSlidingExpiration = true,
//            CookieExpireTimeSpan = TimeSpan.FromHours(2),
//            //RefreshTokenTimeSpan = TimeSpan.FromDays(7),
//            //RefreshTokensEnabled = true,
//            SwaggerUrl = "/custom-swagger",
//            ExternalPages = ["/page1", "/page2"],
//            ConnectionString = "CustomConnectionString",
//            AsymmetricTokenPublicKey_Xml = "<PublicKeyXml>",
//            AsymmetricTokenPrivateKey_Xml = "<PrivateKeyXml>",
//            AsymmetricTokenPublicKey_Pem = "<PublicKeyPem>",
//            AsymmetricTokenPrivateKey_Pem = "<PrivateKeyPem>",
//            AsymmetricAlgorithm = IdGlobalDefaultValues.ASYMETRIC_ALGORITHM,
//            SecurityAlgorithm = "RS256",
//            TokenExpirationMinutes = 60,
//            SymmetricTokenSigningKey = "SymmetricKey",
//            TokenIssuer = "Issuer",
//            UseDbTokenProvider = true,
//            AllowExternalPagesDevModeAccess = true
//        };

//        // Act: Run the Setup method using _fixture
//        _fixture.Setup(setupOptions);

//        // Assert: Verify all fields in IdInfrastructureSettings
//        var fields = typeof(IdInfrastructureSettings)
//            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

//        foreach (var field in fields)
//        {
//            var fieldName = field.Name;

//            // Find the corresponding property in setupOptions
//            var property = typeof(IdInfrastructureSetupOptions)
//                .GetProperty(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

//            Assert.NotNull(property); // Ensure a matching property exists

//            // Get the values from both the field and the property
//            var fieldValue = field.GetValue(null); // Static field, so pass null
//            var propertyValue = property.GetValue(setupOptions);

//            // If the property is a collection or enumerable, compare individual items
//            if (propertyValue is IEnumerable<object> propertyCollection && fieldValue is IEnumerable<object> fieldCollection)
//            {
//                var propertyList = propertyCollection.ToList();
//                var fieldList = fieldCollection.ToList();

//                Assert.Equal(propertyList.Count, fieldList.Count); // Ensure the counts match

//                for (int i = 0; i < propertyList.Count; i++)
//                {
//                    Assert.Equal(propertyList[i], fieldList[i]); // Compare individual items
//                }
//            }
//            else
//            {
//                // Assert that the values match for non-collection properties
//                Assert.NotNull(fieldValue);
//                Assert.Equal(propertyValue, fieldValue);
//            }
//        }
//    }

//}//Cls
