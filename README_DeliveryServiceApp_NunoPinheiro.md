# Delivery Service Exercise - Resolution, Nuno Pinheiro

# Generic information

This exercise exposes a REST API that allows the managing of a Delivery Service based on route paths between different places.<br>
All the CRUD operations to manage the routes are provided, as well as endpoints to return:
- all the route paths, between two places;
- the path, between two places, with the least total time;
- the path, between two places, with the least total cost.

Any client must hold a registration and perform an authentication, in order to consume the Delivery Service API.

### Responses returned by the API:
1. **Successful response, with content:** HTTP status **200**, with the formatted data in the body of the response.
2. **Successful response, without content:** HTTP status **204**, without body.
3. **Unsuccessful response, with error:** HTTP status related with the error. Details related with the error will be given in the body.
#

# Data description

## Entities:

The following entities define the models for the logic of the API:

- `User`: Holds any user's information that consumes the API.
    - `Username`: The combination of a username (unique) and a password (turned into `PasswordHash` and `PasswordSalt`) authenticates the user.
    - `RoleName`: It defines the role that a user holds. Only the users with the role `admin` are able to consume any endpoint in the API. The others can only perform GET requests.

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string RoleName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}
```

- `Role`: Defines the roles allowed to consume the API.
    - Currently, two roles are supported: `admin` and `basic`. A user may also have no role defined; this gives the same rights as being `basic`.
    - Only the users with the role `admin` are able to consume any endpoint in the API. The others can only perform GET requests.

```csharp
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

- `Point`: Defines a place/location in the delivery map, with its unique identifier and name/description.

```csharp
public class Point
{
    public int Id { get; set; }
    public string Description { get; set; }
}
```

- `Step`: Defines a unique connection (with direction) between two points.
    - `Start`: The point where the step starts.
    - `End`: The point where the step ends (the step goes from the start to the end).
    - `Time`: The time that the step takes to be completed.
    - `Cost`: The cost of completing the step.

```csharp
public class Step
{
    public int Id { get; set; }
    public Point Start { get; set; }
    public Point End { get; set; }
    public decimal Time { get; set; }
    public decimal Cost { get; set; }
    public int StartId { get; set; }
    public int EndId { get; set; }
}
```

- `RouteBase`: Defines the basis for a route between two points (the origin and the destination). Between these points, registered as a route, several paths (lists of consecutive connections/steps) may exist, depending on the inserted steps.
    - `Origin`: The point where the route begins.
    - `Destination`: The point where the route ends.

```csharp
public class RouteBase
{
    public int Id { get; set; }
    public Point Origin { get; set; }
    public Point Destination { get; set; }
    public int OriginId { get; set; }
    public int DestinationId { get; set; }
}
```

- `StepsCollection`: Collection of consecutive steps that, in a **route**, defines a **path** between the *start of the first step* (the **origin**) and the *end of the last step* (the **destination**).
    - `Steps`: The list of the consecutive steps.
    - `TotalCost`: The total cost of the collection (the sum of all the steps' costs).
    - `TotalTime`: The total time of the collection (the sum of all the steps' times).
    - Any `StepsCollection` must have, at least, two steps, since a single step between the origin and the destination points is not considered as a valid path. 

```csharp
public class StepsCollection
{
    public List<Step> Steps { get; set; }
    public decimal TotalCost { get; set; }
    public decimal TotalTime { get; set; }
}
```

- `Route`: Relates a `RouteBase` with all the `StepsCollection`s calculated for it.
    - Through the `RouteBase`, an `Origin` and a `Destination` are defined.
    - The paths for this route are computed dinamically, depending on the existing points and steps. Each path connecting the `Origin` and the `Destination` defines a `StepsCollection`.
    - `StepsCollectionList`: Holds all the possible `StepsCollection` connecting the related `Origin` and `Destination`.

```csharp
public class Route : RouteBase
{
    public List<StepsCollection> StepsCollectionList { get; set; }
}
```

## Data values:

A data context is added in the start-up of the app. For simplicity, this defines a temporary in-memory data persistence called `DeliveryServiceDb`, which allows the operationalization of the app.<br>
- The data inserted in the start-up of the app is the data defined in the exercise sheet.
- A predefined `admin` user is also inserted:
    - Username: *nuno.admin*
    - Password: *12345*
    - After authentication, the produced token must be used in the subsequent requests.

Both the operations related with data repositories and business logic are stated in interfaces, which permits different implementations.
#

# Application settings

In the file *appsettings.json*, some custom settings are available, in order to change behaviors within the application:

| Settings Section | Setting  | Meaning | Allowed values    | Default value |
|------------------|----------|---------|-------------------|---------------|
| CacheOptions | UseCache | Use cache or not | true/false (bool) | true |
|              | ExpireTimeMinutes | Cache expiration time (minutes) | double | 5 minutes |
| TokenOptions | Secret | Secret used to generate security tokens | string | --- |
|              | ExpireTimeMinutes | Token expiration time (minutes) | double | 5 minutes |

- When the use of cache is set, the paths computed for requested routes are cached. However, when new points, steps or other elements are created/changed/deleted, these cache entries are removed.

#

# Application exceptions

The API returns custom exceptions for predicted error behaviors. These exceptions are carried in the body of unsuccessful responses.

- For each application custom exception, an error code, a message and a related HTTP status code are provided, as follows:

| Error Code | Message | HTTP status code |
|------------|---------|------------------|
| 1 | Username cannot be null, empty or whitespace. | 422 |
| 2 | Password cannot be null, empty or whitespace. | 422 |
| 3 | The given username is already in use. | 422 |
| 4 | The specified user was not found. | 400 |
| 5 | The defined user's role is not valid. | 400 |
| 6 | Point's description cannot be null, empty or whitespace. | 422 |
| 7 | The given point's description is already in use. | 422 |
| 8 | Specified point was not found. | 400 |
| 9 | A route with the same origin and destination already exists. | 422 |
| 10 | The origin and destination points of a route or step cannot be the same. | 422 |
| 11 | Specified route base was not found. | 400 |
| 12 | A step with the same start and end points already exists. | 422 |
| 13 | Specified step was not found. | 400 |
| 14 | The decimals time and cost of a step must be positive. | 422 |
| 15 | A route with the specified origin and destination does not exist. | 422 |
| 16 | No step corresponding to the beginning of the route was found. | 400 |
| 17 | No step corresponding to the destination of the route was found. | 400 |

#

# Endpoints

The API exposes the following endpoints:

| Route template | Endpoints | Description |
|----------------|-----------|-------------|
| users | POST users/authenticate | Authenticates a registered user. |
|       | POST users/register | Registers a user. |
|       | GET users | Gets collection of users, depending on filters. |
|       | GET users/{id} | Gets user by its identifier. |
|       | PUT users/{id} | Updates the identified user. |
|       | DELETE users/{id} | Deletes the identified user. |
| points | POST points | Creates a new point. |
|        | GET points | Gets collection of points, depending on filters. |
|        | GET points/{id} | Gets point by its identifier. |
|        | PUT points/{id} | Updates the identified point. |
|        | DELETE points/{id} | Deletes the identified point. |
| steps | POST steps | Creates a new step. |
|       | GET steps | Gets collection of steps, depending on filters. |
|       | GET steps/{id} | Gets step by its identifier. |
|       | PUT steps/{id} | Updates the identified step. |
|       | DELETE steps/{id} | Deletes the identified step. |
| routes | POST routes | Creates a new route. |
|        | GET routes | Gets collection of routes, depending on filters. |
|        | GET routes/{id} | Gets route by its identifier. |
|        | PUT routes/{id} | Updates the identified route. |
|        | DELETE routes/{id} | Deletes the identified route. |
| route-steps | GET route-steps/{pointOrigin}/{pointDestination} | Gets all the collections of steps that define valid paths between a given origin and destination points. |
|             | GET route-steps/least-time/{pointOrigin}/{pointDestination} | Gets the collection of steps - between a given origin and destination points - that lasts the least time to complete. |
|             | GET route-steps/least-cost/{pointOrigin}/{pointDestination} | Gets the collection of steps - between a given origin and destination points - that takes the least cost to complete. |

#

# Tests

The tests on the application are written in the project `DeliveryServiceApp.Tests`.

Each group of tests is related with one of the entities that structure the API. The aim of the tests is to go through all the CRUD and algorithmic methods in the business logic, expecting to perform successful operations.

#
