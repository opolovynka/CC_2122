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
# downloading nodejs
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS downloadnodejs
RUN mkdir -p C:\\nodejsfolder
WORKDIR C:\\nodejsfolder
SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop';$ProgressPreference='silentlyContinue';"]
RUN Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v10.16.3/node-v10.16.3-win-x64.zip"; Expand-Archive nodejs.zip -DestinationPath C:\\; Rename-Item "C:\\node-v10.16.3-win-x64" C:\\nodejs

# running build and publish
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY --from=downloadnodejs C:\\nodejs C:\\Windows\\system32
# copy our solution to app folder
COPY ./GoSpeak/ /app
# switch to the folder app
WORKDIR /app
# restore test project dependencies and run tests
RUN npm test

# restore API project and build release/publish builded project ot QuestionService/publish folder
RUN npm run publish
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
We can check our image to run it as container by running this command:

```powershell
docker run --rm -it -p 5000:80 opolovynka/gospeak:last
```
the result is :
```powershell
info: Microsoft.Hosting.Lifetime[0]   
      Now listening on: http://[::]:80
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\app
```

we can see that our application is running and if we will call the link http://localhost:5000/api/questions/ in the browser, we will recive json string with all questions we have regarding to the [API document](https://github.com/opolovynka/GoSpeak/blob/master/docs/API.md)

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

now we can see, when we push something to the repository it initiating the build:
![image](https://user-images.githubusercontent.com/91627367/143450063-1d1b77c9-600f-4eaa-abee-c66a47da96e6.png)

And after it finished we can see the results of the built:
![image](https://user-images.githubusercontent.com/91627367/143450191-3c4952b3-32be-4ee6-8138-74211128b68f.png)

To Configurate buid and publish our image to the Docker Hub, we will use GiHub action:
* for this we have to configurate our secrets for Docker Hub, our login and password, which will be using while we publish. So, go to > Repository> Settings>Secrets
![image](https://user-images.githubusercontent.com/91627367/143485752-ce025fff-b5cf-43bf-9968-d2281fc936b4.png)
![image](https://user-images.githubusercontent.com/91627367/143486590-1df2083c-28a9-4fc0-be48-5ce649db70d5.png)

Then press New repository secret
![image](https://user-images.githubusercontent.com/91627367/143486581-e0f07900-2e92-4e28-a841-6fe37985a0ac.png)
And then we have to add our secrets
![image](https://user-images.githubusercontent.com/91627367/143486689-b97cf33b-bb8e-4946-85d9-342dc9080859.png)

1) Enter secret's name: DOCKER_PASSWORD
2) Enter login for docker: opolovynka in our case
3) Press Add secret

The same we have to do with password:
1) Enter secret's name: DOCKER_USERNAME
2) Enter value for the password
3) Press Add secret.

Now we can use this variables in our github acction:
* Let's create new workflow, go to Actions > New workflow
![image](https://user-images.githubusercontent.com/91627367/143487740-12091faf-a4ea-40f0-b84f-d78916c71ace.png)
now we have to add code of this worklof, we will run it only, if [build and test](https://github.com/opolovynka/GoSpeak/blob/master/docs/Tests.md) passed successefully. So, code will be

```yaml
name: Push Docker image to Docker Hub

on:
  workflow_run:
    workflows: ["Build and Test"]
    types: [completed]

jobs:
  on-success:
    if: ${{ github.event.workflow_run.conclusion == 'success' }}
    name: Push Docker image
    runs-on: ubuntu-latest
    steps:
      - name: Check out the repo
        uses: actions/checkout@v2

      - name: Log in to Docker Hub
        uses: docker/login-action@f054a8b539a109f9f41c372932f1ae047eff08c9
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}

      - name: Extract metadata (tags, labels) for Docker
        id: meta
        uses: docker/metadata-action@98669ae865ea3cffbcbaa878cf57c20bbf1c6c38
        with:
          images: opolovynka/gospeak
          tags: latest

      - name: Build and push Docker image
        uses: docker/build-push-action@ad44023a93711e3deb337508980b4b5e9bcdc5dc
        with:
          context: .
          push: true
          tags: ${{ steps.meta.outputs.tags }}
          labels: ${{ steps.meta.outputs.labels }}
```

So, now if our commit were successfully build and tested, it will be automatically published to Docker hub:
![image](https://user-images.githubusercontent.com/91627367/143488430-46c0fc48-9a95-4895-8663-3004070f8fe5.png)
![image](https://user-images.githubusercontent.com/91627367/143489201-6de38e59-3e8b-47dc-99ba-4b49f277b2d5.png)

In summary we have created Dockerfile wich used to build new image for our application. We configurated **githubactions** to build and publish our image to Docker registry and also configurated Quay Docker registry to build our images.

