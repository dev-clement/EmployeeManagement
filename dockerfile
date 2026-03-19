# Step 1 : Image for the run-time
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Step 2 : SDK for the compilation and tests
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project's file (.csproj) in order to optimize the Docker's cache
COPY ["EmployeeManagement/EmployeeManagement.csproj", "EmployeeManagement/"]

# Restore some dependencies
RUN dotnet restore "EmployeeManagement/EmployeeManagement.csproj"

# Copy rest of the entire code
COPY . .

# Projects build
WORKDIR "/src/EmployeeManagement"
RUN dotnet build "EmployeeManagement.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Step 3 : Run unit tests
FROM build AS test
WORKDIR "/src/EmployeeManagement"
RUN dotnet test "EmployeeManagement.csproj" -c $BUILD_CONFIGURATION --no-build --verbosity normal

# Step 4 : Application delivery
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/EmployeeManagement"
RUN dotnet publish "EmployeeManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Step 5 : Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EmployeeManagement.dll"]