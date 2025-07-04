# Introduction 
TODO: Give a short introduction of your project. Let this section explain the objectives or the motivation behind this project. 

# Getting Started
TODO: Guide users through getting your code up and running on their own system. In this section you can talk about:
1.	Installation process
2.	Software dependencies
3.	Latest releases
4.	API references

# Build and Test

## Visual Studio / Rider
To build the solution, open the `MetaExchange.sln` file and start the build.
Make sure you have the required SDKs and dependencies installed.

There are launch settings for the projects
- `MetaExchange.ConsoleApp` for the console application
- `MetaExchange.WebService` for the web service

To run the unit tests, you can use the Test Explorer in Visual Studio.
Make sure to select the test project `MetaExchange.Tests` and run all tests.

## Docker
Make sure you have Docker and Docker Compose installed on your machine.

To build the Docker image of the MetaExchange.WebService, you can use the following command from the root directory of the solution:

```bash
docker-compose build
```

When the build is complete, you can run the Docker container using the following command:

```bash
docker-compose -f compose.debug.yml up
```

This will start the container defined in the `compose.debug.yml` file.

Finally, you can access the Swagger UI of the MetaExchange.WebService at http://localhost:5075/swagger/index.html in your web browser.
Or send a direct request to the API, e.g. http://localhost:5075/besttrade?tradeType=Buy&cryptoAmount=1