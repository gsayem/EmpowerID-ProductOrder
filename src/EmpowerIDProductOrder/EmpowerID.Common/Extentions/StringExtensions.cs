using System.Text.Json;

namespace EmpowerID.Common.Extentions
{
    public static class StringExtensions
    {
        public static string ToSafeString(this string value)
        {
            return value ?? string.Empty;
        }
        public static bool IsNull(this string value)
        {
            return string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value);
        }
        public static bool IsNotNull(this string value)
        {
            return !value.IsNull();
        }
        public static async Task<T> ReadJsonFileAsync<T>(this string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using FileStream stream = File.OpenRead(filePath);
                    return await JsonSerializer.DeserializeAsync<T>(stream);
                }
                catch (Exception ex)
                {
                    if (ex != null)
                    {

                    }
                }
            }

            return default(T);

        }
    }
}
