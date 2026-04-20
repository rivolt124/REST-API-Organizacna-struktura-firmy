# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY REST-API-Organizacna-struktura-firmy.sln ./
COPY CompanyStructureApi/CompanyStructureApi.csproj CompanyStructureApi/
RUN dotnet restore CompanyStructureApi/CompanyStructureApi.csproj

COPY CompanyStructureApi/ CompanyStructureApi/
RUN dotnet publish CompanyStructureApi/CompanyStructureApi.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CompanyStructureApi.dll"]
