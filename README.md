**Travel Agency API
**
This is a RESTful web API for managing clients and trips in a travel agency system. Built using ASP.NET Core and ADO.NET for database interactions with SQL Server.

**Features**

Create, retrieve, and manage clients.

View all trips and trip details.

Register and unregister clients for trips.

Track registration and payment dates.

Centralized error handling with appropriate HTTP status codes.

**Technologies Used
**
ASP.NET Core 8

ADO.NET with Microsoft.Data.SqlClient

SQL Server

Swagger (OpenAPI)

Endpoints

Clients

GET /clients/{id} - Get client by ID.

POST /clients - Create a new client.

GET /clients/{id}/trips - List all trips associated with a client.

PUT /clients/{idClient}/trips/{tripId} - Register client for a trip.

DELETE /clients/{idClient}/trips/{tripId} - Unregister client from a trip.

Trips

GET /trips - Get all trips.

GET /trips/{id} - Get trip details by ID.

**Status Codes Used
**
200 OK - Successful request.

201 Created - Resource created.

400 Bad Request - Invalid input data.

404 Not Found - Resource not found.

409 Conflict - Business logic conflict (e.g., duplicate registration).

500 Internal Server Error - Unexpected server error.

501 Not Implemented - Placeholder for future functionality.

**Validation
**
Input validation is done both at the model and service level.

Custom exceptions are thrown for not found, conflict, and bad request scenarios.

**Error Handling
**
All exceptions are caught by a global exception handling middleware and returned as a consistent JSON error response:
{
  "status": "Error",
  "message": "An unexpected error occurred.",
  "deitaledMessage": "...",
  "stackTrace": "..."
}

Setup Instructions

Clone the repository.

Configure the database connection string in appsettings.json.

Run the SQL script to create tables: Client, Trip, Client_Trip, Country, Country_Trip.

Build and run the project.

Open Swagger UI to test endpoints at /swagger.

Author

Developed by Patrycja Szpakowska for the APDB course.
