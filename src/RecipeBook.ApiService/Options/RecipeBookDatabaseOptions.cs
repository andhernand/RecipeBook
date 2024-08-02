using System.ComponentModel.DataAnnotations;

namespace RecipeBook.ApiService.Options;

public class RecipeBookDatabaseOptions
{
    public const string Key = "RecipeBookDatabaseOptions";
    [Required] public string DatabaseName { get; set; } = default!;
    [Required] public string CollectionName { get; set; } = default!;
    [Required] public string Username { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;
}