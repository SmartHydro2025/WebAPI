# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["*.csproj", "./"]
RUN dotnet restore

# Copy the rest of the source code and build
COPY . .
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish

# Stage 2: Create the final, smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "WebAPI.dll"]