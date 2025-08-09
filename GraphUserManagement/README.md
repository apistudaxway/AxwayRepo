# Graph User Management API

A .NET 8 Web API built with Clean Architecture for managing users in Azure Entra External ID using Microsoft Graph API.

## 🏗️ Architecture

This project follows Clean Architecture principles with clear separation of concerns:

```
GraphUserManagement/
├── GraphUserManagement.Domain/          # Core business logic and entities
│   ├── Entities/                        # Domain entities (User)
│   ├── ValueObjects/                    # Value objects (Email)
│   ├── Interfaces/                      # Repository interfaces
│   └── Exceptions/                      # Domain-specific exceptions
├── GraphUserManagement.Application/     # Application logic and use cases
│   ├── DTOs/                           # Data Transfer Objects
│   ├── Features/                       # CQRS Commands and Queries
│   ├── Mappers/                        # Entity to DTO mapping
│   └── DependencyInjection.cs         # Service registration
├── GraphUserManagement.Infrastructure/ # External concerns (Graph API)
│   ├── Services/                       # Graph API service implementation
│   ├── Repositories/                   # Repository implementations
│   ├── Mappers/                        # Domain to Graph API mapping
│   ├── Configuration/                  # Configuration models
│   └── DependencyInjection.cs         # Service registration
└── GraphUserManagement.WebApi/         # API controllers and configuration
    ├── Controllers/                    # API controllers
    ├── Program.cs                      # Application startup
    └── appsettings.json               # Configuration
```

## 🚀 Features

- **User Management**: Create, read, update, and delete users in Azure Entra External ID
- **Clean Architecture**: Separation of concerns with clear dependency flow
- **CQRS Pattern**: Command Query Responsibility Segregation using MediatR
- **Validation**: Input validation using FluentValidation
- **Authentication**: JWT Bearer token authentication
- **API Documentation**: Comprehensive Swagger/OpenAPI documentation
- **Logging**: Structured logging throughout the application
- **Health Checks**: Built-in health monitoring endpoints

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Azure Entra External ID tenant
- Azure App Registration with Microsoft Graph permissions

## 🔧 Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd GraphUserManagement
```

### 2. Configure Azure App Registration

1. Go to Azure Portal → Azure Active Directory → App registrations
2. Create a new app registration
3. Add API permissions:
   - Microsoft Graph → Application permissions
   - User.ReadWrite.All
   - Directory.ReadWrite.All
4. Grant admin consent for the permissions
5. Create a client secret

### 3. Configuration

Update the configuration files with your Azure details:

**appsettings.json:**
```json
{
  "GraphApi": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "BaseUrl": "https://graph.microsoft.com",
    "Scopes": "https://graph.microsoft.com/.default",
    "TimeoutInSeconds": 60
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "GraphUserManagementAPI",
    "Audience": "GraphUserManagementAPI",
    "ExpiryInMinutes": 60
  }
}
```

### 4. Build and Run

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd GraphUserManagement.WebApi
dotnet run
```

The API will be available at `https://localhost:7000` (or the port shown in the console).

## 📖 API Documentation

Once the application is running, you can access:

- **Swagger UI**: `https://localhost:7000/` (root URL)
- **OpenAPI Spec**: `https://localhost:7000/swagger/v1/swagger.json`
- **Health Check**: `https://localhost:7000/health`

## 🔐 Authentication

The API uses JWT Bearer token authentication. For testing purposes, you can obtain a token using the auth endpoint:

### Get Authentication Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "tokenType": "Bearer",
  "expiresIn": 3600,
  "expiresAt": "2024-01-01T12:00:00Z"
}
```

### Use Token in Requests

Include the token in the Authorization header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## 📚 API Endpoints

### Users

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/users` | Create a new user | ✅ |
| GET | `/api/users/{id}` | Get user by ID | ✅ |
| GET | `/api/users` | Get users (paginated) | ✅ |

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/login` | Get JWT token | ❌ |

### Health

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/health` | Health check | ❌ |

## 💡 Example Usage

### Create a User

```http
POST /api/users
Authorization: Bearer {your-token}
Content-Type: application/json

{
  "displayName": "John Doe",
  "email": "john.doe@example.com",
  "givenName": "John",
  "surname": "Doe",
  "userPrincipalName": "john.doe@yourdomain.com",
  "jobTitle": "Software Developer",
  "department": "Engineering",
  "mobilePhone": "+1234567890",
  "accountEnabled": true
}
```

### Get a User

```http
GET /api/users/{user-id}
Authorization: Bearer {your-token}
```

## 🔧 Development

### Project Structure

- **Domain Layer**: Contains core business logic, entities, and interfaces
- **Application Layer**: Implements use cases using CQRS pattern with MediatR
- **Infrastructure Layer**: Handles external concerns like Microsoft Graph API
- **Presentation Layer**: ASP.NET Core Web API controllers

### Key Patterns Used

- **Clean Architecture**: Dependency inversion and separation of concerns
- **CQRS**: Command Query Responsibility Segregation
- **Repository Pattern**: Data access abstraction
- **Dependency Injection**: Loose coupling and testability
- **Value Objects**: Domain-driven design principles

### Adding New Features

1. Add domain entities to the Domain layer
2. Create DTOs in the Application layer
3. Implement commands/queries with handlers
4. Add repository interfaces and implementations
5. Create API controllers in the Presentation layer

## 🧪 Testing

### Unit Tests

```bash
# Run unit tests (when implemented)
dotnet test
```

### Integration Testing

Use the Swagger UI or tools like Postman to test the API endpoints.

## 🚀 Deployment

### Environment Variables

For production deployment, consider using environment variables:

```bash
export GraphApi__TenantId="your-tenant-id"
export GraphApi__ClientId="your-client-id"
export GraphApi__ClientSecret="your-client-secret"
export JwtSettings__SecretKey="your-production-secret-key"
```

### Docker (Optional)

Create a Dockerfile for containerization:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GraphUserManagement.WebApi/GraphUserManagement.WebApi.csproj", "GraphUserManagement.WebApi/"]
# Copy other project files...
RUN dotnet restore "GraphUserManagement.WebApi/GraphUserManagement.WebApi.csproj"
COPY . .
WORKDIR "/src/GraphUserManagement.WebApi"
RUN dotnet build "GraphUserManagement.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GraphUserManagement.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GraphUserManagement.WebApi.dll"]
```

## 📝 Configuration Reference

### GraphApi Settings

- `TenantId`: Your Azure AD tenant ID
- `ClientId`: Application (client) ID from app registration
- `ClientSecret`: Client secret from app registration
- `BaseUrl`: Microsoft Graph API base URL
- `Scopes`: OAuth 2.0 scopes for Graph API access
- `TimeoutInSeconds`: HTTP client timeout

### JWT Settings

- `SecretKey`: Secret key for signing JWT tokens (min 32 characters)
- `Issuer`: Token issuer identifier
- `Audience`: Token audience identifier
- `ExpiryInMinutes`: Token expiration time

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

## 🆘 Support

For questions or issues, please:

1. Check the API documentation in Swagger UI
2. Review the logs for error details
3. Ensure proper Azure configuration
4. Verify network connectivity to Microsoft Graph

## 📚 Additional Resources

- [Microsoft Graph Documentation](https://docs.microsoft.com/en-us/graph/)
- [Azure Entra External ID Documentation](https://docs.microsoft.com/en-us/azure/active-directory-b2c/)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)