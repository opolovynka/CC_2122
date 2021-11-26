# Docker
## Dockerfile 
What is Dockerfile you can read [here](https://docs.docker.com/engine/reference/builder/).
* First of all we have to decide wich image will be used to create our image from. As it suggested by [Microsoft](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/net-core-net-framework-containers/official-net-docker-images) :

| Image Name 	| Description 	 | 
|-------|----------------|
| mcr.microsoft.com/dotnet/aspnet:5.0 | ASP.NET Core, with runtime only and ASP.NET Core optimizations, on Linux and Windows (multi-arch) |
| mcr.microsoft.com/dotnet/sdk:5.0 | .NET 5, with SDKs included, on Linux and Windows (multi-arch) |

To build and test our code we will use mcr.microsoft.com/dotnet/sdk:5.0 image which will allow to build our project and run all tests to check if our code is working.

Also to run our scripts for npm we have to install it as well. Based on [this article](https://hyr.mn/docker-dotnet5/) we will add FROM node:lts-buster-slim AS node_base layer to copy nodejs.

```Dockerfile
# nodejs
FROM node:lts-buster-slim AS node_base
#new build layer
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY --from=node_base . .

# copy our solution to app folder
COPY ./GoSpeak/ /app

# add new user to the container
RUN useradd -ms /bin/bash tstuser
#set user as owner of the app folder
RUN chown -R tstuser /app
# set permissions for app folder
RUN chmod 755 /app

USER tstuser
# switch to the folder app
WORKDIR /app
# restore test project dependencies and run tests
RUN npm test
#run tests
CMD ["npm", "test"]
```

* Publish image
To build the image we have to install [Docker Desktop](https://docs.docker.com/desktop/) which will install Docker Engine as well. Then we have to run some terminal for shell and navigate to the solution folder. Then we can run 

```powershell
docker build -t opolovynka/gospeak:last . 
```
we will have out

```powershell
PS C:\Users\alexe\source\repos\Univer\CloudComputing_Fundamentos\CC_2122> docker build -t opolovynka/gospeak:last .
[+] Building 24.0s (15/15) FINISHED
 => [internal] load build definition from Dockerfile                                                                                 0.0s 
 => => transferring dockerfile: 32B                                                                                                  0.0s 
 => [internal] load .dockerignore                                                                                                    0.0s 
 => => transferring context: 2B                                                                                                      0.0s 
 => [internal] load metadata for mcr.microsoft.com/dotnet/sdk:5.0                                                                    0.3s 
 => [internal] load metadata for docker.io/library/node:lts-buster-slim                                                              0.7s 
 => [internal] load build context                                                                                                    0.1s 
 => => transferring context: 22.68kB                                                                                                 0.1s 
 => [node_base 1/1] FROM docker.io/library/node:lts-buster-slim@sha256:a5eecf2ee53935cb7974dfff42260d97289d61be9d7a2062c693be93c0cd  0.0s 
 => [build 1/8] FROM mcr.microsoft.com/dotnet/sdk:5.0@sha256:b2f3f15ee6100efdd36819a429b75d936e4be71bb2487cc48223554f08e11285        0.0s 
 => CACHED [build 2/8] COPY --from=node_base . .                                                                                     0.0s 
 => [build 3/8] COPY ./GoSpeak/ /app                                                                                                 0.1s 
 => [build 5/8] RUN chown -R tstuser /app                                                                                            1.1s 
 => [build 6/8] RUN chmod 755 /app                                                                                                   0.6s 
 => [build 7/8] WORKDIR /app                                                                                                         0.0s 
 => [build 8/8] RUN npm test                                                                                                        19.0s 
 => exporting to image                                                                                                               1.8s 
 => => exporting layers                                                                                                              1.7s 
 => => writing image sha256:aca1b96e4ee75819d3aa4e078e02629f8aa5949104aadbca87f33e69a50347b5                                         0.0s 
 => => naming to docker.io/opolovynka/gospeak:latest                                                                                 0.0s 

Use 'docker scan' to run Snyk tests against images to find vulnerabilities and learn how to fix them
```

We can check our image to run it as container by running this command:

```powershell
docker run opolovynka/gospeak:last
```
the result is :
```powershell
PS C:\Users\alexe\source\repos\Univer\CloudComputing_Fundamentos\CC_2122> docker run opolovynka/gospeak

> GoSpeak@0.2.6 test
> dotnet test Test/Tests.csproj --filter Test=Unit

  Determining projects to restore...
  All projects are up-to-date for restore.
  Model -> /app/Model/bin/Debug/net5.0/GoSpeak.Model.dll
  QuestionService -> /app/QuestionService/bin/Debug/net5.0/GoSpeak.QuestionService.dll
  Tests -> /app/Test/bin/Debug/net5.0/GoSpeak.Tests.dll
Test run for /app/Test/bin/Debug/net5.0/GoSpeak.Tests.dll (.NETCoreApp,Version=v5.0)
Microsoft (R) Test Execution Command Line Tool Version 16.11.0
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 4 ms - /app/Test/bin/Debug/net5.0/GoSpeak.Tests.dll (net5.0)
```
in the line 
```powershell
Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 24 ms
```
we can see that 2 tests were run and they passed ok.

Then we need to publish our container. To do so, we have to decide which registry to choose. Except of Docker hub repository there are plenty of different different registries like:
* [Amazon Elastic Container Registry](https://aws.amazon.com/ecr/) mostly
* [Google Container registry](https://cloud.google.com/container-registry/)
* [Azure Container Registry](https://azure.microsoft.com/en-us/services/container-registry/#overview)
* [GitHub Packaged Docker registry](https://docs.github.com/en/enterprise-server@3.1/packages/working-with-a-github-packages-registry/working-with-the-docker-registry)
* [Quay](https://quay.io/)

we will be using [Docker Hub registry](https://hub.docker.com/) and [Quay](https://quay.io/). I've tried to use Amazon or Google but it was quite complicated, when I've tried Quay, everything passed very easy.
You just need to go to https://quay.io/ and authorize yourself with ReHat account.
Then Quay will suggest to create new repository, which in our case will be public repository with name **gospeak**
![image](https://user-images.githubusercontent.com/91627367/143447191-f06688ca-9656-4270-9411-06f65ad34e3d.png)

also we choose to link our repository to the GitHub repository ![image](https://user-images.githubusercontent.com/91627367/143447310-98e5e4b7-f7f8-459c-90e0-7c1d090c6dab.png)
 which will allow us to configure trigger for make our image when we push to the GitHub repository. After we authorize to the GitHub repository, we will setup build trigger:
![image](https://user-images.githubusercontent.com/91627367/143447484-92129381-27cf-4080-9195-ae996c7d57e4.png)
![image](https://user-images.githubusercontent.com/91627367/143447632-60a9108c-1b3f-447e-9c40-2b8e61e35ed7.png)
1) We will triggering our rebuild action for all branches
2) We will use 'latest' tag for our built
3) and we have select Dockerfile in our repository
![image](https://user-images.githubusercontent.com/91627367/143447837-9a486497-0bbb-4118-8ded-430d042131b4.png)

4) Also we point to the context in which our build will be starting
5) and that's it, press Continue

Then you will see main page of your repository which will be showing you created trigger:
![image](https://user-images.githubusercontent.com/91627367/143448053-f50e8c0a-d830-47d1-8813-6fcf54b9d75d.png)

now we can see, when we push something to the repository it initiating the build:
![image](https://user-images.githubusercontent.com/91627367/143450063-1d1b77c9-600f-4eaa-abee-c66a47da96e6.png)

And after it finished we can see the results of the built:
![image](https://user-images.githubusercontent.com/91627367/143450191-3c4952b3-32be-4ee6-8138-74211128b68f.png)

To Configure build and publish our image to the Docker Hub, we will use GiHub action:
* for this we have to configure our secrets for Docker Hub, our login and password, which will be using while we publish. So, go to > Repository> Settings>Secrets
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

Now we can use this variables in our github action:
* Let's create new workflow, go to Actions > New workflow
![image](https://user-images.githubusercontent.com/91627367/143487740-12091faf-a4ea-40f0-b84f-d78916c71ace.png)
now we have to add code of this workflow, we will run it only, if [build and test](https://github.com/opolovynka/GoSpeak/blob/master/docs/Tests.md) passed successfully. So, code will be

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

In summary we have created Dockerfile which used to build new image for our application. We configured **githubactions** to build and publish our image to Docker registry and also configured Quay Docker registry to build our images.
