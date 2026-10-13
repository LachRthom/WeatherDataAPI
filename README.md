# Weather Data API

A RESTful Web API built with ASP.NET Core (.NET 8) and MongoDB for storing and querying weather sensor data. Built as a guided assignment for my Diploma of IT (Advanced Programming) at TAFE Queensland, 2024.

## What it does

- Stores weather readings from sensor devices: temperature, precipitation, humidity, wind, pressure, solar radiation and location
- Lets users query readings by ID, device or date range
- Reports the maximum temperature and precipitation recorded, plus a snapshot of a device's latest readings
- Manages users with role-based access (Teacher, Student, Sensor)

## Tech

- C# / ASP.NET Core Web API (.NET 8)
- MongoDB (MongoDB.Driver)
- Swagger / OpenAPI (Swashbuckle) for documentation and testing

## Design

- **Controllers** handle HTTP requests and validation
- **Repository pattern** with interfaces separates data access from the API logic
- **DTOs** shape the data sent to and from the API
- **Custom API key attribute** checks each request's key and the user's role before allowing access
- Input validation with appropriate status codes (400, 401, 403, 404, 500)

## Endpoints

**Data points** (`/api/DataPoint`)

| Method | Route | Purpose |
|---|---|---|
| GET | `/{id}` | Get a single reading |
| GET | `/{deviceName}/range` | Readings for a device within a date range |
| GET | `/range` | All readings within a date range |
| POST | `/record` | Add a reading |
| POST | `/multiple records` | Add several readings |
| PUT | `/{id}` | Update a reading |
| PATCH | `/{id}/precipitation` | Update precipitation only |
| GET | `/max-temperature` | Highest temperature recorded |
| GET | `/max-precipitation` | Highest precipitation recorded |
| GET | `/snapshot/{deviceName}` | Latest snapshot for a device |

**Users** (`/api/UserData`)

| Method | Route | Purpose |
|---|---|---|
| GET | `/{id}` | Get a user |
| POST | `/` | Create a user (generates an API key) |
| DELETE | `/{id}` | Delete a user |
| DELETE | `/delete-by-role/{role}/from/{startDate}/to/{endDate}` | Bulk delete users by role and date |
| PATCH | `/update-access-level/from/{startDate}/to/{endDate}/to-role/{newRole}` | Bulk change user roles |

## Running it locally

1. Install the .NET 8 SDK and MongoDB Community Server
2. Make sure MongoDB is running on `mongodb://localhost:27017/` (set in `appsettings.json`)
3. Open `WeatherDataAPI.sln` in Visual Studio and run the project
4. Swagger opens in your browser. Click **Authorize** and enter a valid API key to test endpoints

## Known limitations

This was a learning project. Things I'd improve in a production version:

- Hash passwords instead of storing them as plain text, and stop returning them in API responses
- Make database calls async
- Add unit tests
- Tidy the `multiple records` route to remove the space
