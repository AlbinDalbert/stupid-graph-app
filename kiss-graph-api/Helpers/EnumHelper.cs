using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

public static class EnumHelper
{
    /// <summary>
    /// Gets the string value defined in the EnumMember attribute, or the enum name if not found.
    /// </summary>
    public static string GetEnumMemberValue<T>(T enumValue) where T : Enum
    {
        var type = typeof(T);
        var member = type.GetMember(enumValue.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<EnumMemberAttribute>();
        return attribute?.Value ?? enumValue.ToString();
    }

    /// <summary>
    /// Parses a string to an enum value using the EnumMember attribute or enum name.
    /// </summary>
    public static T ParseEnumMemberValue<T>(string value) where T : Enum
    {
        var type = typeof(T);
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if ((attribute?.Value == value) || field.Name.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null);
            }
        }

        throw new ArgumentException($"Unknown value '{value}' for enum '{type.Name}'");
    }
}
