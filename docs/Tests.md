# Tests

***

First of all we had to decided which Framework to use for our tests. Based on this article [NUnit vs. XUnit vs. MSTest: Comparing Unit Testing Frameworks In C#](https://www.lambdatest.com/blog/nunit-vs-xunit-vs-mstest/) desision was made that selected Framework will be XUnit because: 
 * It provides the best isolation level of tests
 * The framework can be extended in many ways which allows us to customize functionality which will be needed for our tests.

# Test runner

To run tests we use [xUnit test runner](https://github.com/dotnet/coreclr.xunit) which allow us to run tests in independed way and see all results clear.
<br/>Also we're going to use github actions. 
For this we have to create file .github/workflows/test.yml <img title="" src="https://user-images.githubusercontent.com/91627367/142263700-9cd64127-8973-43af-bca5-e2bd3f976662.png" alt="" align="right" width="500">
 whith next script: name: Build and Test

```C#
name: Build and Test

on: push

jobs:
  build:
    name: Build
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2
      - run: dotnet build GoSpeak/GoSpeak.sln
    
  tests:
    name: Unit Testing
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v2.1.0
      - run: dotnet test GoSpeak/Tests/Tests.csproj --filter Test=Unit
```

# Task runner

As task runner will be used npm. In this [article](https://blog.teamtreehouse.com/use-npm-task-runner) we can see how npm can be used in many ways.
In our case we will have package.js file with next code:
```JSON
{
  "name": "GoSpeak",
  "version": "0.2.5",

  "scripts": {
    "setup": "npm install && dotnet build",
    "test": "dotnet test",
    "setup_all": "npm run setup && npm run test"
  },
  "testRunner": "dotnet-test-xunit",
  "dependencies": {
    "xunit": "2.1.0-*",
    "dotnet-test-xunit": "2.1.0-*"
  }
}
```
So, now if we will run **'npm test'** we will have the next results:
![image](https://user-images.githubusercontent.com/91627367/140401585-36b168e0-18c4-4380-8183-3fb8381c0316.png)

We have run 2 tests and they are successfully passed.
