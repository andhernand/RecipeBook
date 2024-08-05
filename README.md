# Recipe Book

An online application for storing Recipes.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

## Getting Started

### Clone the Repository

```bash
git clone git@github.com:andhernand/RecipeBook.git
cd RecipeBook
```

### Setup Environment Variables

Create a `.env` file in the root directory of the project. This file contains the required environment variables for building and running the application and tests.

```text
RECIPE_BOOK_API_USER=""
RECIPE_BOOK_API_PWD=""
RECIPE_BOOK_API_DATABASE=""
RECIPE_BOOK_API_COLLECTION=""
```

The environment variables that start with `RECIPE_BOOK_API` need to match the values in the [appsettings.json](src/RecipeBook.ApiService/appsettings.json) file.

### Install Aspire, Restore, Build, and Run

*NOTE:* Make sure Docker Desktop is running

```bash
dotnet workload install aspire
dotnet restore RecipeBook.sln
dotnet build RecipeBook.sln --no-restore
dotnet run --project src/RecipeBook.AppHost/RecipeBook.AppHost.csproj
```

### Running Tests

Tests utilize [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/) to orchestrate the creation of necessary dependencies for all tests. To run the tests, use the following command:

*NOTE:* Make sure Docker Desktop is running

```bash
dotnet test RecipeBook.sln
```

## Contributing

Contributions are welcome! Please fork this repository and submit a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
