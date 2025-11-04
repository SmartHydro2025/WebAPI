# Stage 1: The build environment, using the .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# CORRECTED: Copy the project file first and restore its dependencies
COPY SmartHydro_API.csproj ./
RUN dotnet restore "SmartHydro_API.csproj"

# Copy the rest of the application's source code
COPY . .

# CORRECTED: Publish the specific project
RUN dotnet publish "SmartHydro_API.csproj" -c Release -o /app/publish --no-restore


# Stage 2: The final runtime image, using the lean ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Set the URL the app will listen on within the container
ENV ASPNETCORE_URLS=http://+:8080

# Expose port 8080 to the outside world
EXPOSE 8080

# Define the entry point for the container.
ENTRYPOINT ["dotnet", "SmartHydro_API.dll"]