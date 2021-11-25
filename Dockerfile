# downloading nodejs
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS downloadnodejs
RUN mkdir -p nodejsfolder
WORKDIR /nodejsfolder
SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop';$ProgressPreference='silentlyContinue';"]
RUN Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v10.16.3/node-v10.16.3-win-x64.zip"; Expand-Archive nodejs.zip -DestinationPath .; Rename-Item "node-v10.16.3-win-x64" nodejs

# running build and publish
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY --from=downloadnodejs nodejsfolder/nodejs /Windows/system32
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