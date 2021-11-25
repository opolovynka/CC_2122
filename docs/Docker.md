# Docker
## Dockerfile 
What is Dockerfile you can read [here](https://docs.docker.com/engine/reference/builder/).
* First of all we have to decide wich image will be used to create our image from. As it suggested by [Microsoft](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/net-core-net-framework-containers/official-net-docker-images) :

| Image Name 	| Description 	 | 
|-------|----------------|
| mcr.microsoft.com/dotnet/aspnet:5.0 | ASP.NET Core, with runtime only and ASP.NET Core optimizations, on Linux and Windows (multi-arch) |
| mcr.microsoft.com/dotnet/sdk:5.0 | .NET 5, with SDKs included, on Linux and Windows (multi-arch) |

To build and test our code we will use mcr.microsoft.com/dotnet/sdk:5.0 image which will allow to build our project and run all tests to check if our code is working.
To run application we will use mcr.microsoft.com/dotnet/aspnet:5.0 image and it will be our last layer of the image.

```Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY ./GoSpeak/ /app

WORKDIR /app
RUN dotnet restore Tests/Tests.csproj
RUN dotnet test Tests/Tests.csproj --filter Test=Unit
 
RUN dotnet restore QuestionService/QuestionService.csproj && dotnet publish QuestionService/QuestionService.csproj -c Release -o QuestionService/publish

COPY GoSpeak/data.json QuestionService/publish/data/


#create a new layer using the cut-down aspnet runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
# copy over the files produced when publishing the service
COPY --from=build app/QuestionService/publish  ./
# expose port 80 as our application will be listening on this port
EXPOSE 5000
# run the web api when the docker image is started
ENTRYPOINT ["dotnet", "/app/GoSpeak.QuestionService.dll"]
```
