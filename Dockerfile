# Step 1: Build frontend (Vue)
FROM node:20 AS frontend
WORKDIR /frontend
COPY project/ .
RUN npm install && npm run build

# Step 2: Build backend (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY manufacturing_system/ ./manufacturing_system/

# 複製前端打包好的檔案到 wwwroot
COPY --from=frontend /frontend/dist ./manufacturing_system/wwwroot

WORKDIR /src/manufacturing_system
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Step 3: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ManufacturingSystem.dll"]
