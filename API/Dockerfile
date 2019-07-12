FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
LABEL author="Devil"
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
#ENV ConnectionString=Server=LHRLT-0999\\SQLSERVER\\Catalog=Heroes_DB;Database=Heroes_DB;User=SA;Password=abcd@1234
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Tour-Of-Heroes-Server.dll"]