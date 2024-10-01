namespace RecipeBook.ApiService;

public class ApiEndpoints
{
    private const string ApiBase = "api";

    public static class Recipes
    {
        private const string Base = $"{ApiBase}/recipes";
        public const string Create = Base;
        public const string GetAll = Base;
        public const string Update = Base;
        public const string GetById = $"{Base}/{{id:regex(^[0-9a-fA-F]{{{{24}}}}$)}}";
        public const string Delete = $"{Base}/{{id:regex(^[0-9a-fA-F]{{{{24}}}}$)}}";
        public const string Tag = "Recipes";
    }
}