namespace RecipeBook.AppHost
{
    internal static class Utilities
    {
        internal static string GetEnvironmentVariableOrThrow(string variableName)
        {
            return Environment.GetEnvironmentVariable(variableName) ??
                   throw new InvalidOperationException($"Environment variable not set: {variableName}");
        }
    }
}
