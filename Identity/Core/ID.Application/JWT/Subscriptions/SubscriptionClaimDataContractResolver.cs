using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StringHelpers;
using System.Reflection;

namespace ID.Application.JWT.Subscriptions;


public class SubscriptionClaimDataContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty property = base.CreateProperty(member, memberSerialization);

        // Apply Camel Case naming:
        property.PropertyName = property.PropertyName?.ToCamelCase(); // Or use your preferred camel case conversion

        // Handle private setters:
        if (!property.Writable)
            property.Writable = true;

        return property;
    }
}

