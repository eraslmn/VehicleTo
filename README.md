# VehicleTo - Vehicle Management Microservice

VehicleTo is a vehicle management microservice project designed to manage various aspects of vehicle operations. The project consists of four distinct web APIs, each serving a unique purpose within the overall system. This project is primarily focused on creating unit tests for the different components, making it a valuable resource for software testing.

# Project Structure

The VehicleTo project is organized into four main web APIs:

1. Reservations API -
This API handles all operations related to vehicle reservations. It ensures that all reservation data is accurately managed and stored.

2. Customers API -
The Customers API manages customer-related data and operations. It is crucial for maintaining customer records and ensuring seamless customer interactions.

3. Vehicle API -
The Vehicle API focuses on managing vehicle data, with endpoints for adding, updating, deleting, and retrieving vehicle details. This API is essential for maintaining an up-to-date inventory of vehicles.

4. Gateway API -
The Gateway API acts as an entry point for the other APIs, routing requests to the appropriate API based on the request type. This API is crucial for ensuring that the system is scalable and can handle various types of requests efficiently.

# Project Aim

The primary goal of this project is to create comprehensive unit tests for the Reservations, Customers, and Vehicle APIs. These tests are designed to ensure that each component functions correctly and reliably. The focus on unit testing makes this project a valuable resource for learning and implementing software testing best practices.

# Note on Azure Service Bus

Initially, the Customers and Reservations APIs utilized Azure Service Bus for queuing operations. However, the Azure Service Bus has since been deleted. Despite this, the project's main aim remains to develop and maintain unit tests for the various components. This focus ensures that the APIs are thoroughly tested and any issues are identified and resolved promptly.

# Technology Stack

Web APIs: Developed using .NET Core 6 &
Unit Tests: Written using .NET Core 7

# Starting the Project

To run the VehicleTo project, you need to set it up as <multiple startup projects> in your IDE. This ensures that all necessary APIs are started simultaneously, allowing the project to function correctly.

# Conclusion

VehicleTo is a robust microservice project aimed at managing various vehicle-related operations. By focusing on unit testing, the project ensures high reliability and maintainability of the APIs. Despite the removal of the Azure Service Bus, the project's primary goal of creating and maintaining comprehensive unit tests remains intact.

