# 🍷 MyWine - Application de Gestion de Cave à Vin

## Description
Application de gestion et suivi de cave à vin avec scraping de prix et recommandations personnalisées.

## Architecture
- **Backend**: .NET 8 Web API
- **Frontend**: À venir (React/Angular/Vue.js)
- **Base de données**: PostgreSQL (à venir)
- **Tests**: xUnit avec Moq

## Structure du projet
```
MyWine/
├── src/
│   ├── WineCellar.Api/          # API Web
│   ├── WineCellar.Core/         # Entités et interfaces
│   └── WineCellar.Infrastructure/ # Repositories et services
├── tests/
│   └── WineCellar.Tests/        # Tests unitaires
└── .github/workflows/           # GitHub Actions
```

## Commandes utiles
```bash
# Build du projet
dotnet build

# Lancer les tests
dotnet test

# Lancer l'API
dotnet run --project src/WineCellar.Api
```

## Fonctionnalités
- ✅ CRUD des bouteilles de vin
- ✅ Système de notation (0-100)
- ✅ Calcul de la valeur totale de la cave
- 🔄 Scraping des prix (en cours)
- 🔄 Recommandations personnalisées (en cours)

## CI/CD
Le pipeline GitHub Actions s'exécute sur chaque push et pull request :
- ✅ Build de la solution
- ✅ Exécution des tests unitaires
- ✅ Vérification de la qualité du code
- ❌ Blocage des merges si les tests échouent