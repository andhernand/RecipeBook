using FluentValidation;

using MongoDB.Bson;

using RecipeBook.ApiService.Regex;

namespace RecipeBook.ApiService.Validators;

public class ObjectIdValidator : AbstractValidator<string>
{
    public ObjectIdValidator()
    {
        RuleFor(x => x)
            .Must(ValidateObjectId)
            .OverridePropertyName("Id")
            .WithMessage("'Id' is not in the correct format.");
    }

    private static bool ValidateObjectId(string id)
    {
        return RegexPatterns.HexIdRegex().IsMatch(id)
               && ObjectId.TryParse(id, out _);
    }
}