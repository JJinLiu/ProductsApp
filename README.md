# Welcome to Product Application
## About the project
This is a .NET web API project. It includes the APIs with the ability to create, update, delete, read and search products.

### Structure
* The projected was designed by 4 layers. 
* Controller layer is holding the APIs and mapping Dto to Entities. 
* Service layer is holding business logics and validations. 
* Repository layer talks to database.
* Model layer is holding the entity models and creating the DB context.
* Services are unit tested

### Built with
* .Net Core
* C#
* In-Memory database

### Get started
* Debug ProductsApplication project in Visual Studio and the Swagger page will be opened automatically.
* Or in Powershell you can navigate to `\ProductsApp\ProductsApplication` and run the command line `Dotnet run` 
* Once the project is running, you can test the API with either Postman and Swagger
