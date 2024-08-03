namespace RecipeBook.ApiService;

public class ApiEndpointConstants
{
    private const string ApiBase = "api";

    public static class Recipes
    {
        private const string Base = $"{ApiBase}/recipes";
        public const string Create = Base;
        public const string GetAll = Base;
        public const string GetById = $"{Base}/{{id}}";
        public const string Update = $"{Base}/{{id}}";
        public const string Delete = $"{Base}/{{id}}";
        public const string Tag = "Recipes";
    }
}