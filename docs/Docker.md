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
# copy our solution to app folder
COPY ./GoSpeak/ /app
# switch to the folder app
WORKDIR /app
# restore test project dependencies and run tests
RUN dotnet restore Tests/Tests.csproj
RUN dotnet test Tests/Tests.csproj --filter Test=Unit

# restore API project and build release/publish builded project ot QuestionService/publish folder
RUN dotnet restore QuestionService/QuestionService.csproj && dotnet publish QuestionService/QuestionService.csproj -c Release -o QuestionService/publish
#copy data file which will be used for seed data
COPY GoSpeak/data.json QuestionService/publish/data/


#create a new layer using the cut-down aspnet runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
WORKDIR /app
# copy over the files produced when publishing the service
COPY --from=build app/QuestionService/publish  ./
# expose ports 5000 and 50001 as our application will be listening on this port
EXPOSE 5000
EXPOSE 5001
# run the web api when the docker image is started
ENTRYPOINT ["dotnet", "/app/GoSpeak.QuestionService.dll"]
```

* Publish image
To build the image we have to install [Docker Desktop](https://docs.docker.com/desktop/) which will install Docker Ingine as well. Then we have to run some terminal for shell and nvaigate to the solution folder. Then we can run 

```powershell
docker build -t opolovynka/gospeak:last . 
```
we will have out

```powershell
PS C:\Users\alexe\source\repos\Univer\CloudComputing_Fundamentos\CC_2122> docker build -t opolovynka/gospeak:last .
Sending build context to Docker daemon   12.6MB
Step 1/13 : FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
 ---> 9e7e9cb04357
Step 2/13 : COPY ./GoSpeak/ /app
 ---> e471222c865a
Step 3/13 : WORKDIR /app
 ---> Running in 465cfc401b88
Removing intermediate container 465cfc401b88
 ---> 70baa614ccba
Step 4/13 : RUN dotnet restore Tests/Tests.csproj
 ---> Running in 595378385c27
  Determining projects to restore...
  Restored C:\app\QuestionService\QuestionService.csproj (in 3.87 sec).
  Restored C:\app\Model\Model.csproj (in 17 ms).
  Restored C:\app\Tests\Tests.csproj (in 8.25 sec).
Removing intermediate container 595378385c27
 ---> 7a6cc3d3cc6b
Step 5/13 : RUN dotnet test Tests/Tests.csproj --filter Test=Unit
 ---> Running in 4e65120a8c93
  Determining projects to restore...
  All projects are up-to-date for restore.
  Model -> C:\app\Model\bin\Debug\net5.0\GoSpeak.Model.dll
  QuestionService -> C:\app\QuestionService\bin\Debug\net5.0\GoSpeak.QuestionService.dll
  Tests -> C:\app\Tests\bin\Debug\net5.0\GoSpeak.Tests.dll
Test run for C:\app\Tests\bin\Debug\net5.0\GoSpeak.Tests.dll (.NETCoreApp,Version=v5.0)
Microsoft (R) Test Execution Command Line Tool Version 16.11.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 24 ms - GoSpeak.Tests.dll (net5.0)
Removing intermediate container 4e65120a8c93
 ---> ec8d406fed7d
Step 6/13 : RUN dotnet restore QuestionService/QuestionService.csproj && dotnet publish QuestionService/QuestionService.csproj -c Release -o QuestionService/publish
 ---> Running in 3ef1de405133
  Determining projects to restore...
  All projects are up-to-date for restore.
Microsoft (R) Build Engine version 16.11.2+f32259642 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Model -> C:\app\Model\bin\Release\net5.0\GoSpeak.Model.dll
  QuestionService -> C:\app\QuestionService\bin\Release\net5.0\GoSpeak.QuestionService.dll
  QuestionService -> C:\app\QuestionService\publish\
Removing intermediate container 3ef1de405133
 ---> 181657650762
Step 7/13 : COPY GoSpeak/data.json QuestionService/publish/data/
 ---> 75e9971876ea
Step 8/13 : FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
 ---> c33fcdbcf9e5
Step 9/13 : WORKDIR /app
 ---> Using cache
 ---> adf02f549819
Step 10/13 : COPY --from=build app/QuestionService/publish  ./
 ---> Using cache
 ---> b6d68242f51b
Step 11/13 : EXPOSE 5000
 ---> Using cache
 ---> 0c90aec24ba3
Step 12/13 : EXPOSE 5001
 ---> Running in 47c7929d32ab
Removing intermediate container 47c7929d32ab
 ---> 8fda3185b6da
Step 13/13 : ENTRYPOINT ["dotnet", "/app/GoSpeak.QuestionService.dll"]
 ---> Running in 0987dcda9cc7
Removing intermediate container 0987dcda9cc7
 ---> 405dcc4f66ad
Successfully built 405dcc4f66ad
Successfully tagged opolovynka/gospeak:last

Use 'docker scan' to run Snyk tests against images to find vulnerabilities and learn how to fix them
command, which will build a new image for us.
```

in the line 
```powershell
Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 24 ms
```
we can see thet 2 tests were run and they passed ok.
Then we need to publish our container. To do so, we have to decide which registry to choose. Except of Docker hub repository there're plenty of different different registries like:
* [Amazon Elastic Container Registry](https://aws.amazon.com/ecr/) mostly
* [Google Container registry](https://cloud.google.com/container-registry/)
* [Azure Container Registry](https://azure.microsoft.com/en-us/services/container-registry/#overview)
* [GitHub Packaged Docker registry](https://docs.github.com/en/enterprise-server@3.1/packages/working-with-a-github-packages-registry/working-with-the-docker-registry)
* [Quay](https://quay.io/)

we will be using [Docker Hub registry](https://hub.docker.com/) and [Quay](https://quay.io/). I've tried to use Amazon or Google but it was quite complicated, when I've tried Quay, everything passed very easy.
You just need to go to https://quay.io/ and authorize yourself with ReHat account.
Then Quay will suggest to create new repository, which in our case will be public repository with name **gospeak**
![image](https://user-images.githubusercontent.com/91627367/143447191-f06688ca-9656-4270-9411-06f65ad34e3d.png)

also we choosed to link our repository to the GitHub repository ![image](https://user-images.githubusercontent.com/91627367/143447310-98e5e4b7-f7f8-459c-90e0-7c1d090c6dab.png)
 which will allow us to configurate trigger for make our image when we push to the GitHub repository. After we authorize to the GitHub repository, we will setup build trigger:
![image](https://user-images.githubusercontent.com/91627367/143447484-92129381-27cf-4080-9195-ae996c7d57e4.png)
![image](https://user-images.githubusercontent.com/91627367/143447632-60a9108c-1b3f-447e-9c40-2b8e61e35ed7.png)
1) We will triggering our rebuild action for all branches
2) We will use 'latest' tag for our built
3) and we have select Dockerfile in our repostitory
![image](https://user-images.githubusercontent.com/91627367/143447837-9a486497-0bbb-4118-8ded-430d042131b4.png)
4) Also we point to the context in which our build will be starting
5) and that's it, press Continue

Then you will see main page of your repository which will be showing you created trigger:
![image](https://user-images.githubusercontent.com/91627367/143448053-f50e8c0a-d830-47d1-8813-6fcf54b9d75d.png)

