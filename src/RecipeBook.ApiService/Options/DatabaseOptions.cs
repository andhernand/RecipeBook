using System.ComponentModel.DataAnnotations;

namespace RecipeBook.ApiService.Options;

public class DatabaseOptions
{
    public const string Key = "DatabaseOptions";
    [Required] public string DatabaseName { get; set; } = default!;
    [Required] public string CollectionName { get; set; } = default!;
    [Required] public string Username { get; set; } = default!;
    [Required] public string Password { get; set; } = default!;
}