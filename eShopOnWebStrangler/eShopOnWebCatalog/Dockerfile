# # Use the official .NET Core SDK as a parent image
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /app

# # Copy the project file and restore any dependencies
# # COPY *.csproj ./
# COPY . .
# RUN dotnet restore

# # Copy the rest of the application code

# # Publish the application
# RUN dotnet publish -c Release -o out

# # Build the runtime image
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app
# COPY --from=build /app/out ./

# # Set environment to run on port 80

# # Expose the port
# EXPOSE 8080

# # Start the application
# ENTRYPOINT ["dotnet", "eShopOnWebCatalog.dll"]



# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
#COPY ["src/PublicApi/PublicApi.csproj", "./PublicApi/"]
#RUN dotnet restore "./PublicApi/PublicApi.csproj"
#COPY . .
# WORKDIR "/app"
RUN dotnet restore

RUN dotnet build "./eShopOnWebCatalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./eShopOnWebCatalog.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "eShopOnWebCatalog.dll"]
