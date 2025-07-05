# Introduction 
The MetaExchange solution analyzes the order books of a set of exchanges and
outputs a set of order recommendations to execute against these order books
in order to buy/sell a specified amount of cryptocurrency
at the lowest/highest possible price (i.e. best trade).

The solution consists of the following projects:
- `MetaExchange.ConsoleApp`: A console application that allows users find the best trade.
   Trade type (Buy, Sell) and cryptocurrency amount can be specified either interactive or
   via command line arguments. The order books are read from a set of JSON files in a specified folder.
- `MetaExchange.WebService`: A web service that that allows users find the best trade.
   Trade type (Buy, Sell) and cryptocurrency amount can be specified as URL parameters.
   The order books are read from a set of JSON files in a specified folder.
- `MetaExchange.Tests`: A test project that contains unit tests for the MetaExchange.Domain library.
- `MetaExchange.Domain`: A common library that contains the pure core functionality to find the best trade
   from the order books of a set of exchanges.
- `MetaExchange.Infrastructure`: A common library that contains infrastructure functionality, e.g. to read
   exchange data from JSON files and to convert it into domain objects.

# Implementation Details

## Solution structure

The solution is implemented in C# and uses .NET 9 as the target framework.

Solution structure:
![Solution structure](/Documentation/MetaExchange.ProjectStructure.png)

## Best Trade Algorithm

The solution implements a greedy algorithm to find the best trade.

The following diagram illustrates the algorithm's behavior for buying
a specified amount of cryptocurrency at the lowest possible price.

To achieve this, the algorithm analyzes the Asks (sell orders) in the order books of the exchanges.

![Algorithm](/Documentation/MetaExchange.Algorithm.png)

First, it sorts the Asks by price in ascending order.
Then, it iterates through the sorted list of Asks (starting with the lowest price)
and tries to accumulate the specified amount of cryptocurrency.
For each Ask, it generates an order recommendation by checking:
- Does it have to buy the full amount of cryptocurrency from this Ask, or just a part?
- Does it have enough Euro on this exchange to buy the amount?
  - If not, it buys as much as possible from this Ask and moves to the next Ask.

After processing all Asks, the algorithm returns a list of order recommendations and
indicates whether the specified amount of cryptocurrency was fully bought or not.

In the diagram, the green boxes represent the order recommendations for a specific example:
- the first two Asks from both exchanges were bought fully
- the second Ask from Exchange 2 was partially bought because there was not enough Euro on the exchange anymore
- the second Ask from Exchange 1 was bought fully
- the third Ask from Exchange 1 was partially bought because the specified amount of cryptocurrency was already reached
- all other Asks were skipped

For selling a specified amount of cryptocurrency at the highest possible price, the algorithm is similar.

# Usage

## Console Application

To run the console application, you can use command line arguments like the following:
```bash
MetaExchange.ConsoleApp.exe --order-type Buy --crypto-amount 0.27 --root-folder-path ..\..\..\..\ExampleData\exchanges
```

By omitting the command line arguments, you will be prompted to enter the order type (Buy/Sell) and the amount of cryptocurrency interactively:
```bash
MetaExchange.ConsoleApp.exe --root-folder-path ..\..\..\..\ExampleData\exchanges
```

To learn more about the command line arguments of the console application, please type:
```bash
MetaExchange.ConsoleApp.exe --help
```
## Web Service

The web service provides a REST API to find the best trade.

Example of a request to the web service:
```http
GET http://localhost:5075/besttrade?tradeType=Buy&cryptoAmount=10
```

# Build and Test

## Visual Studio / Rider
To build the solution, open the `MetaExchange.sln` file and start the build.
Make sure you have the required SDKs and dependencies installed.

There are launch settings for the projects
- `MetaExchange.ConsoleApp` for the console application
- `MetaExchange.WebService` for the web service

Simply select the desired run/debug configuration in Visual Studio or Rider and start debugging.

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