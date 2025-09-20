# =============================
# Step 1: Build frontend (Vue)
# =============================
FROM node:20 AS frontend
WORKDIR /frontend
COPY project/ .
RUN npm install
RUN npm run build

# =============================
# Step 2: Build backend (ASP.NET Core)
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製 ASP.NET Core 專案
COPY manufacturing_system/. ./manufacturing_system/

# 切換工作目錄到專案
WORKDIR /app/manufacturing_system

# 還原 NuGet 套件
RUN dotnet restore

# 複製前端打包好的檔案到 wwwroot
COPY --from=frontend /frontend/dist ./wwwroot

# 發佈為 Release
RUN dotnet publish -c Release -o /app/publish

# =============================
# Step 3: Runtime image
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "ManufacturingSystem.dll"]
