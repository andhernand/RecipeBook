using System.Text.RegularExpressions;

namespace RecipeBook.ApiService.Regex;

public static partial class RegexPatterns
{
    [GeneratedRegex("^[0-9a-fA-F]{24}$", RegexOptions.Compiled, 5)]
    public static partial System.Text.RegularExpressions.Regex HexIdRegex();
}