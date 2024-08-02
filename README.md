# Recipe Book

An online application for storing Recipes.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
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
MONGODB_INITDB_ROOT_USERNAME=""
MONGODB_INITDB_ROOT_PASSWORD=""
RECIPE_BOOK_API_USER=""
RECIPE_BOOK_API_PWD=""
RECIPE_BOOK_API_DATABASE=""
RECIPE_BOOK_API_COLLECTION=""
```

### Start MongoDB Server with Docker

Make sure Docker Desktop is running, then execute the following command to start the MongoDb server container:

```bash
docker-compose up -d
```

### Restore, Build, and Run

```bash
dotnet restore RecipeBook.sln
dotnet build RecipeBook.sln --no-restore
dotnet run --project src/RecipeBook.Api/RecipeBook.Api.csproj
```

### Integration Testing

Integration tests utilize [Testcontainers](https://dotnet.testcontainers.org/) to create a MongoDD database for all integration tests. To run the tests, use the following command:

```bash
dotnet test RecipeBook.sln
```

## Contributing

Contributions are welcome! Please fork this repository and submit a pull request with your changes.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.
