# ============ Build Stage ============
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["EcommerceWebApi.sln", "."]
COPY ["EcommerceWepApi.API/EcommerceWepApi.API.csproj", "EcommerceWepApi.API/"]
COPY ["EcommerceWepApi.BLL/EcommerceWepApi.BLL.csproj", "EcommerceWepApi.BLL/"]
COPY ["EcommerceWepApi.DAL/EcommerceWepApi.DAL.csproj", "EcommerceWepApi.DAL/"]

# Restore packages
RUN dotnet restore "EcommerceWepApi.API/EcommerceWepApi.API.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/EcommerceWepApi.API"
RUN dotnet publish -c Release -o /app/publish --no-restore

# ============ Runtime Stage ============
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create directory for SQLite database
RUN mkdir -p /data && chmod 777 /data

# Copy published files
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DB_PATH=/data

EXPOSE 8080

ENTRYPOINT ["dotnet", "EcommerceWepApi.API.dll"]
