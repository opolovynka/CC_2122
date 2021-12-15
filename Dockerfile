#new build layer
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

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
# to install next dependencies we have to create manifest
RUN dotnet new tool-manifest
# restore test project dependencies and run tests
RUN dotnet tool install Nake --version 3.0.0-beta-01

RUN dotnet Nake build

CMD ["dotnet", "Nake", "test"]