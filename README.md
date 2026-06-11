# LinguaSwap Backend

Backend API for **LinguaSwap**, a vocabulary practice app for learning languages.

This API handles authentication, user libraries, vocabulary items, practice sessions, attempts, and progress statistics.

## Features

* User registration and login
* JWT authentication
* Public demo libraries
* Private user libraries
* Vocabulary item management
* Term creation, update and deletion
* Practice session creation
* Practice attempts with correctness feedback
* User progress statistics
* Protected endpoints for authenticated users

## Tech Stack

* .NET
* ASP.NET Core Web API
* Entity Framework Core
* JWT Bearer Authentication
* Relational database
* Scalar / OpenAPI for API testing and documentation

## Project Structure

The backend follows a layered structure.

```txt
src/
  LinguaSwap.Api/
    Controllers/
    Program.cs
    appsettings.json

  LinguaSwap.Application/
    Auth/
    Libraries/
    Practice/
    Progress/
    Vocab/

  LinguaSwap.Infrastructure/
    Data/
    Persistence/
    Migrations/
```

Adjust this section if your actual project folders are different.

## Getting Started

### Prerequisites

You need:

* .NET SDK
* A running database
* The LinguaSwap frontend app if you want to test the full flow

### Restore dependencies

```bash
dotnet restore
```

### Configure the application

Create or update your local configuration file.

For local development, you can use `appsettings.Development.json` or user secrets.

Example configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_DATABASE_CONNECTION_STRING"
  },
  "Jwt": {
    "Issuer": "LinguaSwap",
    "Audience": "LinguaSwap",
    "Secret": "YOUR_LOCAL_DEVELOPMENT_SECRET",
    "ExpiresInHours": 12
  }
}
```

Do not commit real secrets or production connection strings.

## Database

If the project uses Entity Framework Core migrations, apply migrations with:

```bash
dotnet ef database update
```

If your startup project and infrastructure project are separate, you may need a command like:

```bash
dotnet ef database update --project src/LinguaSwap.Infrastructure --startup-project src/LinguaSwap.Api
```

Adjust project paths depending on your solution structure.

## Run the API

```bash
dotnet run --project src/LinguaSwap.Api
```

The API will run on the configured ASP.NET Core URL.

Common local URLs are:

```txt
https://localhost:7001
http://localhost:5000
```

## API Documentation

When running locally, API documentation/testing may be available through Scalar or OpenAPI, depending on the backend configuration.

Common URLs:

```txt
/scalar
/openapi/v1.json
/swagger
```

## Main API Areas

### Auth

| Method | Endpoint             | Description             |
| ------ | -------------------- | ----------------------- |
| `POST` | `/api/auth/register` | Register a new user     |
| `POST` | `/api/auth/login`    | Login and receive a JWT |

### Libraries

| Method | Endpoint                           | Description                         |
| ------ | ---------------------------------- | ----------------------------------- |
| `GET`  | `/api/libraries/public`            | Get public demo libraries           |
| `GET`  | `/api/libraries`                   | Get current user libraries          |
| `POST` | `/api/libraries`                   | Create a private library            |
| `GET`  | `/api/libraries/{libraryId}/items` | Get vocabulary items from a library |

### Vocabulary

| Method   | Endpoint                         | Description              |
| -------- | -------------------------------- | ------------------------ |
| `POST`   | `/api/vocab/items`               | Create a vocabulary item |
| `DELETE` | `/api/vocab/items/{vocabItemId}` | Delete a vocabulary item |
| `PUT`    | `/api/vocab/terms/{termId}`      | Update a term            |
| `DELETE` | `/api/vocab/terms/{termId}`      | Delete a term            |

### Practice

| Method | Endpoint                                      | Description              |
| ------ | --------------------------------------------- | ------------------------ |
| `POST` | `/api/practice/sessions`                      | Start a practice session |
| `GET`  | `/api/practice/sessions/{sessionId}/next`     | Get the next word        |
| `POST` | `/api/practice/sessions/{sessionId}/attempts` | Submit an answer         |

### Progress

| Method | Endpoint                     | Description                           |
| ------ | ---------------------------- | ------------------------------------- |
| `GET`  | `/api/progress/summary`      | Get progress summary                  |
| `GET`  | `/api/progress/history`      | Get progress history                  |
| `GET`  | `/api/progress/by-language`  | Get progress grouped by language      |
| `GET`  | `/api/progress/by-pair`      | Get progress grouped by language pair |
| `GET`  | `/api/progress/top-mistakes` | Get most missed vocabulary items      |

## Authentication

Protected endpoints require a JWT access token.

The frontend sends the token using the `Authorization` header:

```txt
Authorization: Bearer <token>
```

The token contains user identity information and is used by the backend to restrict access to private resources.

## Practice Flow

1. The user selects a public or private library.
2. The frontend starts a practice session:

```txt
POST /api/practice/sessions
```

3. The backend creates a practice session.
4. The frontend asks for the next word:

```txt
GET /api/practice/sessions/{sessionId}/next
```

5. The user submits an answer:

```txt
POST /api/practice/sessions/{sessionId}/attempts
```

6. The backend stores the attempt and returns whether the answer was correct.

## Progress Statistics

The progress endpoints provide a first version of user statistics:

* Total attempts
* Correct attempts
* Accuracy
* Practiced words
* History by day
* Stats by language
* Stats by language pair
* Top mistakes

## Related Repositories

* Frontend app: `https://github.com/carlostrcs/Linguaswap.Web`

## Current Status

This project is currently an MVP backend for LinguaSwap. The main API flows are implemented, but there are still planned improvements.

## Planned Improvements

* Refresh token authentication
* Better expired token handling
* More complete validation rules
* Improved progress statistics
* More user-friendly error responses
* Better support for empty libraries
* Prevent invalid vocabulary states
* Add more tests
* Improve query performance for large datasets
