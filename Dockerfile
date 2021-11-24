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