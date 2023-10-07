using System.Globalization;

namespace Common.Utils
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLowerInvariant();
            }

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            var yourString = str.ToLower().Replace("_", " ");
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            yourString = info.ToTitleCase(yourString).Replace(" ", string.Empty);
            return yourString;
        }
    }
}