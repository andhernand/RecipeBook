using FluentValidation;

using RecipeBook.Contracts.Requests;

namespace RecipeBook.ApiService.Validators;

public class CreateRecipeRequestValidator : AbstractValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator()
    {
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
}