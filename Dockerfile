#new build layer
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build

COPY ./GoSpeak /app

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
RUN dotnet new tool-manifest --force

# restore test project dependencies and run tests
#RUN dotnet tool install Nake --version 3.0.0-beta-01 --tool-path ~/bin

RUN dotnet tool install --tool-path ~/bin Nake --version 3.0.0-beta-01
#RUN export PATH="$PATH:/home/tstuser/bin"

ENV PATH="$PATH:/home/tstuser/bin"

CMD ["nake", "test"]
#CMD ["/bin/sh"]