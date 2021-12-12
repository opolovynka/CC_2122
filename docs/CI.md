#  CI (Continuous Integration)
Continuous Integration is conception which allows us to check are all pieces of code integrated to each other. For this purpose we can use different systems. Let's try and analyze some of them.

## Travis
I've tried to run CI on Travis but I guess it was hard to realize. First I couldn't receive confirmation email and couldn't run any build. After few days I've received the confirmation email but then I couldn't make the diferent builds for different versions of dotnet works. I've recieved this error: 
![image](https://user-images.githubusercontent.com/91627367/145724864-6af49b30-79a0-4993-b223-480326d6792e.png)
![image](https://user-images.githubusercontent.com/91627367/145724871-98d52a45-76ef-4325-a50d-c7a73fb75fee.png)
![image](https://user-images.githubusercontent.com/91627367/145724882-8613f587-bc7c-4767-b019-e56602f3cb5e.png)

So, I've decided to work with another one.

## AppVeyor
There is good CI for .net projects [AppVeyor](https://ci.appveyor.com). It was very easy to configure - what you have to do is just gitve access to the github repository:
![image](https://user-images.githubusercontent.com/91627367/145724972-a28b8af9-e66a-4ebc-bc8a-1b095f723256.png)
but then I've found out that it [doesn't support GitHub Checks API](https://github.com/appveyor/ci/issues/2311), so this system was declined.

## CircleCI
Circle CI seemed fine for my needs it supports GigHub Checks API and you can use docker images to run your build, so it was sound promising for me.
* First of all we have to [SignUp](https://circleci.com/signup/) with our GitHub account:
![image](https://user-images.githubusercontent.com/91627367/145725122-4b5479e0-0327-4b92-8417-0b81be76297e.png)
![image](https://user-images.githubusercontent.com/91627367/145725156-21dcedc3-3950-46c5-ad37-4d4044220c60.png)
* Then you will be redirecting to the Welcome page:
![image](https://user-images.githubusercontent.com/91627367/145725202-b0126b13-da05-4c1a-ad82-b3d20a8ebc51.png)
    1. Fill purpose of your project;
    2. What role do you have (software engineer, student, etc..)
    3. How do you plan to use CircleCI (Evaluate CircleCI as CI/CD tool)
    4. What the size of your organization (1-5)

* Then you will be invited to select project (repository) for which you want to configure CI/CD, you can choose to use existed config.yml file of create new one and then click 'Setup project' ![image](https://user-images.githubusercontent.com/91627367/145725615-080b43cd-b359-40f4-94ca-76b7022c84bf.png)
* Then you have to select platform/language of your project for my project I have to select .net
![image](https://user-images.githubusercontent.com/91627367/145725781-b6286cfb-73de-411c-874c-adb9eb775b8d.png)
* It will be created new file .circleci/config.yml for your repository, let's setup it correctly, the code of the config file is:
```yml
version: 2.1

workflows:
  version: 2
  test:
    jobs:
      - build-netstandard-2
      - test-netcore:
          name: .NET Standard 2.0 + .NET Core 2.1
          docker-image: mcr.microsoft.com/dotnet/core/sdk:2.1-focal
          build-target-framework: netstandard2.0
          test-target-framework: netcoreapp2.1
          requires:
            - build-netstandard-2
      - test-netcore:
          name: .NET Standard 2.0 + .NET Core 3.1
          docker-image: mcr.microsoft.com/dotnet/core/sdk:3.1-focal
          build-target-framework: netstandard2.0
          test-target-framework: netcoreapp3.1
          requires:
            - build-netstandard-2
      - test-netcore:
          name: .NET Standard 2.0 + .NET 5.0
          docker-image: mcr.microsoft.com/dotnet/sdk:5.0-focal
          build-target-framework: netstandard2.0
          test-target-framework: net5.0
          requires:
            - build-netstandard-2
      - test-netcore:
          name: .NET Standard 2.0 + .NET 6.0
          docker-image: mcr.microsoft.com/dotnet/sdk:6.0-focal
          build-target-framework: netstandard2.0
          test-target-framework: net6.0
          requires:
            - build-netstandard-2

orbs:
  win: circleci/windows@1.0.0

jobs:
  build-netstandard-2:
    parameters:
      build-target-framework:
        type: string
        default: netstandard2.0
    docker:
      - image: mcr.microsoft.com/dotnet/core/sdk:2.1-focal
    environment:
      ASPNETCORE_SUPPRESSSTATUSMESSAGES: true
    steps:
      - checkout
      - restore_cache:
          keys:
            - dotnet-packages-v1-{{ checksum "GoSpeak/QuestionService/QuestionService.csproj" }}
      - run:
          name: "Install project dependencies"
          command: dotnet restore GoSpeak/QuestionService/QuestionService.csproj
      - save_cache:
          key: dotnet-packages-v1-{{ checksum "GoSpeak/QuestionService/QuestionService.csproj" }}
          paths:
            - /root/.nuget/packages
      - run:
         name: "Build Application according to some given configuration"
         command: dotnet build  GoSpeak/QuestionService/QuestionService.csproj --configuration Release
      - persist_to_workspace:
          root: GoSpeak/QuestionService
          paths:
            - bin
            - obj

  test-netcore:
    parameters:
      docker-image:
        type: string
      build-target-framework:
        type: string
      test-target-framework:
        type: string
    docker:
      - image: <<parameters.docker-image>>
    environment:
      ASPNETCORE_SUPPRESSSTATUSMESSAGES: true
      TESTFRAMEWORK: <<parameters.test-target-framework>>
    steps:
      - checkout
      - attach_workspace:
          at: GoSpeak/QuestionService
      - restore_cache:
          keys: # see comments under build-netstandard-2
            - dotnet-packages-v1-{{ checksum "GoSpeak/Test/Tests.csproj" }}
      - run:
          name: "Install project dependencies"
          command: dotnet restore GoSpeak/Test/Tests.csproj
      - save_cache:
          key: dotnet-packages-v1-{{ checksum "GoSpeak/Test/Tests.csproj" }}
          paths:
            - /root/.nuget/packages
      - run:
          name: "Run Application Tests"
          command: dotnet test GoSpeak/Test/Tests.csproj --filter Test=Unit -f <<parameters.test-target-framework>>
```
which will allow us to check different language versions. What we can see [here](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/configure-language-version) 
| Target framework | version | C# language version default |
|------------------|---------|-----------------------------|
| .NET | 6.x | C# 10 |
| .NET | 5.x | C# 9.0 |
| .NET Core |	3.x | C# 8.0 |
| .NET Core |	2.x |	C# 7.3 |
|           |     |        |

When we save our config file and on the Dashboard we will see ![image](https://user-images.githubusercontent.com/91627367/145726110-7028981d-4e6e-4652-afb5-12541b1671ad.png), which show us that our project is working for previouse versions.
Now we also have to configure GitHub Checks API, for that we have to go to the project settings > organization settings > VCS:
![image](https://user-images.githubusercontent.com/91627367/145726187-7142f830-a074-4619-a6a3-cea1d83d6af2.png)
![image](https://user-images.githubusercontent.com/91627367/145726276-b12c5445-e993-4119-9d04-ca31a09b69fc.png)
![image](https://user-images.githubusercontent.com/91627367/145726297-543febf0-896e-4f84-87cb-15b3f8a133cf.png)

Then press Manage GitHub Checks and you will be redirected to GitHub access page, after confirm you will see:
![image](https://user-images.githubusercontent.com/91627367/145726358-923347b5-7fe5-41b5-b183-1e6168eb016f.png)
That's it, now we have everything ready.

## GitHub Actions



