# Software-Management-Core
End goal for this project is a flexible setup of a CQRS system with eventsourcing, that can be applied to both small and very large projects easily with a variety of technologies.

The main application business logic, including api and datalayer(s) implementations are all in this repository's solution for now, for convenience. This is intended to be split up later to prevent a huge repository download (that happens far too often). Components should stand on their own.

## Setup
This requires Visual Studio 2017 RC or higher with .NET Core 1.1.0 or higher and an EntityFrameworkCore compatible backing for the database (set to SQL for now, others can be easily added). Right now you need to modify the connectionstring in the project directly. This will be made configurable in future.

Windows
1. Install the latest Visual Studio 2017 (RC) if you haven't already. You only need the web module with aspnetcore for running this. 
2. If you want to contribute, clone the project to your own github account, and then clone it locally from there. Otherwise, just clone the project locally (on Windows I recommend using GitHub Desktop for convencience).
3. Install a SQL Sever Community or Developer edition if you haven't already (or reconfigure the project to use localdb, SQLLite, etc.) and create a database called SoftwareManagement
4. Open the project and change the connection string in both the api and entityframeworkcore projects. 
5. Open the Package Manager console in Visual Studio, select the EntityFrameworkCore project, and run Update-Database to create and/or migrate the database to the latest version. 

You should now be ready to run the project (I prefer using Kestrel rather than IIS Express, it's faster and it gives immediate feedback on what's happening in its console)

### Controller
CommandController is the main controller, which receives a list of CommandDtos from the front-end (see the Angular 2 repo). This is an AspNetCore project which picks up the commands which have a Name property which indicates the type of command, and an Entity property which indicates the entity (aggregate root) the command is for. 

### Command Manager
The CommandManager will determine which Command object matches that combination of Name and Entity, which can be configured in the CommandManager with CommandConfigs. This will route the command to the correct service (e.g. ProductService) which can execute the command. It can route all commands for a specific entity to a (Micro)Service, but also override a specific command for that entity to another service if necessary.

### (Micro)Services
Each service as well as the commands defines its repository and state interfaces, which have to be injected. This ensures that the services are as agnostic as possible about what kind of datastore backs them, which is an important goal for this project (being able to easily implement and experiment with different types of datastore, both as a learning exercise and as an important practical approach where different datastores can be mixed and matched purposefully). The services (and most helpers like IDateTimeProvider) are also written as Shared Projects, which don't have any dependency of their own and are therefore easily packaged in any platform (so could be used as .NET Framework 4.5 client dlls, Xamarin Forms, targeting different .NET Standard versions, etc.) They are all bundeled into a single SoftwareManagementCore project/dll, but are intended such that at a minimum each 'domain' (projects, products, contacts etc.) can be published as a separate dll.
For more information on Shared Projects as used here, read this blog: https://www.linkedin.com/pulse/i-shared-projects-arwin-van-arum
For more information on how the services interact with the repository for state management, read this blog: https://www.linkedin.com/pulse/i-repository-arwin-van-arum
Determining what the barriers are for the services follows many of the DDD principles, where each aggregate root gets its own service. In addition, as an anti-corruption method, each relationship between aggregate roots is its own service. E.g. contacts and companies are separate entities that can have (both over time and in shape of contracting) many to many relationships, which are defined in the Employment service. 

### Api
The AspNetCore (api) project uses the native dependency injection to inject the implementation of these state and repository interfaces, and currently one such implementation is included in the form of an EntityFrameworkCore implementation (that currently expects a Database on the localhost named SoftwareManagement that it can log into with TrustedAuthentication - the connectionstring will be configurable later). 

### Command Processing to Repository
Currently the CommandController treats the list of commands that comes in as one transaction, and both stores the commands and executes the commands immediately, which results in updated products. This can later be separated if necessary, e.g. save the commands and/or throw them on a bus, and then let other processes have at them. But one of the main goals is to show that even in simple scenarios, this can be a nice and efficient way of working. And of course ideally we will update the Angular 2 data locally and then sync with commands in a webworker thread in the background.

### Command/Event Sourcing
The commands are saved to a repository rather than events. As a part of the DDD approach where everything should be ubiquitous language, commands should hold universally from UI to backend as representing the intent of an action. The services are setup so that they can always interpret the commands and translate them into implementations that lead to state changes, communicated to an interface. The injected repository implementations of that interface are responsible for the actual projection. So should you setup a projection on a different server, all you need to do is include the dll that holds the service's Shared Projects you want to use there, and setup the command manager to receive and route the commands there, and then your implementation of the repository interface can respond and create the state you need. This project should get multiple examples of such an implementation soon.

In addition, the intent is to add an example of recreating objects from their commands using an in-memory state repository should you need full Event Sourcing capabilities such as being able to reconstruct historical data (which is the only one that truly needs being able to rebuild objects from their commands, as far as I am currently aware - most other scenarios can use non-realtime projections just as well I imagine). 

### Hooking up to Client
The client will send commands to the CommandsController, as a simple json array. For an example, see the Angular 2 Client project (https://github.com/niwrA/Software-Management-Angular2-Client). Each epic also gets its own readonly controller (see ProjectsController for an example), and then we can start experimenting with different kinds of efficient datastore backings for different purposes, as introducing a new one will only require playing back the commands that are already stored in the database, so that should be fun. Of course it should be extremely easy to implement other clients as well, so other examples will follow (UWP, Xamarin, React, etc.)

### Testing
Testing aims to use proper Unit testing, e.g. the system under test will only be tested for core functionality and implementing the declared interfaces, following the 70-20-10 principle. The Unit Tests are the most important. They test the logic of all public methods and whether they properly implement their interfaces, and if you can't do this cleanly, your code probably leans to a ball of mud. This project uses xUnit (because of early, good Core support) with Moq (most popular in .NET but I hadn't used it yet, and I like how the mock exposes setup and verify), and a setup from Microsoft for Controller testing. Finally, the repository is tested using in-memory databases, for speed. I started with Microsoft's InMemory database, but the EntityCoreFramework database is using relations (to show you can still work 'old-school') and InMemory doesn't support them. For this reason I've switched to SQLLite's inmemory database, which is also very fast and so far very satisfactory. It could end up being an interesting in-memory caching service option at some point.

If you contribute, I strongly recommend trying to work test first, and work forward, starting with creating the command coming in from the UI. For each step, just think how your new command/method should interact with the service, state and repository interfaces.
