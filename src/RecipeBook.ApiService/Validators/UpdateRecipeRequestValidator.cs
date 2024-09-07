using FluentValidation;

using RecipeBook.ApiService.Regex;
using RecipeBook.Contracts.Requests;

namespace RecipeBook.ApiService.Validators;

public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .OverridePropertyName("Id")
            .WithMessage("'Id' must not be empty")
            .DependentRules(() =>
            {
                RuleFor(x => x.Id)
                    .Must(ValidateObjectId)
                    .OverridePropertyName("Id")
                    .WithMessage("The provided 'Id' is not valid");
            });

        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Ingredients)
            .NotEmpty();

        RuleForEach(x => x.Ingredients)
            .NotEmpty()
            .WithMessage("Ingredient at index {CollectionIndex} must not be empty.");

        RuleFor(x => x.Directions)
            .NotEmpty();

        RuleForEach(x => x.Directions)
            .NotEmpty()
            .WithMessage("Direction at index {CollectionIndex} must not be empty.");
    }

    private static bool ValidateObjectId(string id)
    {
        return RegexPatterns.HexIdRegex().IsMatch(id);
    }
}