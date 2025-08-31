# MyWine - Wine Cellar Management Application

[![CI/CD Pipeline](https://github.com/VOTRE-USERNAME/MyWine/actions/workflows/ci.yml/badge.svg)](https://github.com/VOTRE-USERNAME/MyWine/actions/workflows/ci.yml)

## Description

Wine cellar management and tracking application with price scraping and personalized recommendations.

## Architecture

- **Backend**: .NET 8 Web API
- **Frontend**: Coming soon (React/Angular/Vue.js)
- **Database**: PostgreSQL (coming soon)
- **Tests**: xUnit with Moq

## Project structure

```text
MyWine/
├── src/
│   ├── WineCellar.Api/          # Web API
│   ├── WineCellar.Core/         # Entities and interfaces
│   └── WineCellar.Infrastructure/ # Repositories and services
├── tests/
│   └── WineCellar.Tests/        # Unit tests
└── .github/workflows/           # GitHub Actions
```

## Useful commands

```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Run the API
dotnet run --project src/WineCellar.Api
```

## Features

- CRUD for wine bottles
- Rating system (0-100)
- Calculate total cellar value
- Price scraping (in progress)
- Personalized recommendations (in progress)

## CI/CD

The GitHub Actions pipeline runs on each push and pull request:

- Build the solution
- Run unit tests
- Verify code quality
- Block merges if tests fail
