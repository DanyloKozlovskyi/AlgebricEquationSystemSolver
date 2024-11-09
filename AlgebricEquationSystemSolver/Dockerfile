FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AlgebricEquationSystemSolver/AlgebricEquationSystemSolver.WEBApi.csproj", "AlgebricEquationSystemSolver/"]
COPY ["AlgebricEquationSystemSolver.DataAccess/AlgebricEquationSystemSolver.DataAccess.csproj", "AlgebricEquationSystemSolver.DataAccess/"]
RUN dotnet restore "./AlgebricEquationSystemSolver/AlgebricEquationSystemSolver.WEBApi.csproj"
COPY . .
WORKDIR "/src/AlgebricEquationSystemSolver"
RUN dotnet build "./AlgebricEquationSystemSolver.WEBApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AlgebricEquationSystemSolver.WEBApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AlgebricEquationSystemSolver.WEBApi.dll"]