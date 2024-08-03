using System.Text.Json.Serialization;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RecipeBook.ApiService.Models;

public class Recipe
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;

    [BsonElement("title")]
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    [BsonElement("description")]
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [BsonElement("ingredients")]
    [JsonPropertyName("ingredients")]
    public required IEnumerable<string> Ingredients { get; init; } = [];

    [BsonElement("directions")]
    [JsonPropertyName("directions")]
    public required IEnumerable<string> Directions { get; init; } = [];
}