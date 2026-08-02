using System.Text.RegularExpressions;

namespace MovieDatabase.Model
{
    public static class WildcardMatcher
    {
        public static bool IsMatch(string? value, string? pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return true;
            }

            if (value == null)
            {
                return false;
            }

            string regexPattern = string.Join(".*", pattern.Split('*').Select(Regex.Escape));
            return Regex.IsMatch(value, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
