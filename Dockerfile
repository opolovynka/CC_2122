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

CMD ["npm", "test"]