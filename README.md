# TodoApi

ASP.NET Core 8.0 Web API for managing Todo items, built following Microsoft's tutorial with additional enhancements including AutoMapper, repository pattern, PostgreSQL database, and comprehensive testing.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)

## Overview

This is a Todo management Web API that allows users to:
- Create new Todo items
- View all Todo items
- View Todo items by id
- Update existing Todo items
- Delete Todo items

This project follows and extends the ASP.NET Core Web API tutorial:
- [Create a web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio-code)

**Additional enhancements made:**
- PostgreSQL database integration instead of in-memory database
- AutoMapper for object mapping
- Repository pattern implementation
- Unit and integration testing
- Configuration with user secrets

## Features

- **CRUD Operations**: Create, Read, Update, and Delete Todo items
- **Database**: PostgreSQL database with Entity Framework Core
- **AutoMapper**: Object-to-object mapping for clean DTOs
- **Swagger/OpenAPI**: Interactive API documentation
- **Test coverage**: Unit tests with NUnit and Moq, Integration tests with TestContainers
- **CORS Support**: Cross-origin resource sharing enabled

## Technologies Used

- **Framework**: ASP.NET Core 8.0 Web API
- **Database**: 
  - PostgreSQL (Development & Production)
  - Entity Framework Core 9.0
- **Object Mapping**: AutoMapper 15.0
- **Testing Framework**: 
  - NUnit 4.3
  - Moq 4.20
  - TestContainers for PostgreSQL
- **Documentation**: Swagger/OpenAPI
- **Development Tools**:
  - Visual Studio Code
  - Entity Framework Migrations
  - User Secrets for configuration

## Prerequisites

Before running this application, ensure you have:
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [PostgreSQL](https://www.postgresql.org/download/) (for local development)
- Docker (for running integration tests with TestContainers)

## Installation

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd TodoApi
   ```

2. **Install Entity Framework tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Configure connection string**:
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=TodoApiDb;Username=your_username;Password=your_password"
   ```
   
   > **Replace** `your_username` and `your_password` with your actual PostgreSQL credentials.

5. **Create and update the database**:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Running the Application

1. **Using .NET CLI**:
   ```bash
   dotnet run
   ```

2. **Access Swagger Documentation**:
   - Navigate to `http://localhost:5289/swagger`

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/TodoItems` | Get all todo items |
| GET | `/api/TodoItems/{id}` | Get a specific todo item |
| POST | `/api/TodoItems` | Create a new todo item |
| PUT | `/api/TodoItems/{id}` | Update an existing todo item |
| DELETE | `/api/TodoItems/{id}` | Delete a todo item |

## Testing

### Unit Tests
Run unit tests using NUnit:
```bash
dotnet test TodoApi.Tests.Unit
```

### Integration Tests
Run integration tests (uses TestContainers for PostgreSQL):
```bash
dotnet test TodoApi.Tests.Integration
```

### Run All Tests
```bash
dotnet test
```