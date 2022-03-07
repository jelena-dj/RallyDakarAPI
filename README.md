# Rally Dakar Simulation
REST API in ASP.NET CORE (.NET 5)

Hosted on IIS Express - Swagger documentation: https://localhost:44371/swagger/index.html

In-memory database is used. Test data (one race with 6 different vehicles) is seeded upon the start for easier testing.



#Notes for testing

Vehicle Types: Car - 0; Motorcycle - 1; Truck - 2

Vehicle Subtypes: NoSubType - 0; SportsCar - 1; TerrainCar - 2; CrossMotorcycle - 3; SportsMotorcycle - 4

Vehicle Status: pending - 0; HeavyMalfunction - 1, LightMalfunction - 2; Running - 3; Finished - 4

Race Status: Pending - 0, Started - 1, Finished - 2
