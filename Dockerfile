# syntax=docker/dockerfile:1
FROM scratch AS feed

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY --from=feed / /.feed/
COPY . .
RUN --mount=type=secret,id=github_token,required=false \
    if [ -s /run/secrets/github_token ]; then \
      dotnet nuget add source https://nuget.pkg.github.com/marcelo-roman/index.json --name github --configfile nuget.config \
        --username marcelo-roman --password "$(cat /run/secrets/github_token)" --store-password-in-clear-text; \
    fi \
    && dotnet publish src/EventBank.Accounts.Api/EventBank.Accounts.Api.csproj --configuration Release --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
ENV ASPNETCORE_HTTP_PORTS=8080
WORKDIR /app
COPY --from=build /app .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "EventBank.Accounts.Api.dll"]
