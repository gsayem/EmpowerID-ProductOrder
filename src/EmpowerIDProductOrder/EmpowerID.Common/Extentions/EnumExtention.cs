using System.Reflection;
using System.Runtime.Serialization;

namespace EmpowerID.Common.Extentions
{
    public static class EnumExtentions
    {
        public static T ParseEnum<T>(this int value) where T : Enum
        {
            var enumType = typeof(T);
            if (value.IsEnum<T>())
                return (T)Enum.ToObject(enumType, value);
            throw new ArgumentOutOfRangeException(
                $"The number {value} is not valid for enum of type {enumType.FullName}");
        }

        public static T ParseEnum<T>(this string value) where T : struct
        {
            const bool ignoreCase = true;
            T result;
            var defaultValue = default(T);
            //if (!Enum.TryParse(value, ignoreCase, out result)) {
            //    throw new ArgumentOutOfRangeException($"Invalid {typeof(T).Name} enum value '{value}'");
            //}

            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            var enumType = typeof(T);
            foreach (var enumName in Enum.GetNames(enumType))
            {
                var fieldInfo = enumType.GetField(enumName);
                var enumMemberAttribute = ((EnumMemberAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumMemberAttribute), true)).FirstOrDefault();
                if (enumMemberAttribute?.Value.ToSafeString().ToUpper() == value.ToUpper())
                {
                    return Enum.TryParse(enumName, ignoreCase, out result) ? result : defaultValue;
                }
            }

            return Enum.TryParse(value, true, out result) ? result : defaultValue;

        }

        public static bool IsEnum<T>(this int value) where T : Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }
        public static bool IsEnum<T>(this string value) where T : Enum
        {
            return Enum.IsDefined(typeof(T), value);
        }
        public static string GetEnumMemberValue<T>(this T value) where T : struct
        {
            string returnValue = null;
            returnValue = typeof(T)
                .GetTypeInfo()
                .DeclaredMembers
                .SingleOrDefault(x => x.Name == value.ToString())
                ?.GetCustomAttribute<EnumMemberAttribute>(false)
                ?.Value;
            return returnValue.ToSafeString();
        }
        public static List<string> GetEnumMemberValueList<T>() where T : struct, Enum
        {
            var returnValue = typeof(T)
                .GetTypeInfo()
                .DeclaredFields
                .Where(f => f.IsStatic && f.IsPublic && f.FieldType == typeof(T))
                .Select(f => (field: f, attrib: f.GetCustomAttribute<EnumMemberAttribute>()))
                .Where(t => (t.attrib?.IsValueSetExplicitly ?? false) && t.attrib.Value.IsNotNull())
                .Select(s => s.attrib.Value);

            return returnValue.ToList();
        }
    }

}
