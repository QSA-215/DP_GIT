FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /source

COPY *.sln .
COPY ./Valuator/*.csproj ./Valuator/
COPY ./MessageBroker/*.csproj ./MessageBroker/
COPY ./DBController/*.csproj ./DBController/
RUN dotnet restore ./Valuator/Valuator.csproj

COPY ./Valuator ./Valuator/
COPY ./MessageBroker ./MessageBroker/
COPY ./DBController  ./DBController/

RUN dotnet publish ./Valuator/Valuator.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Valuator.dll"]