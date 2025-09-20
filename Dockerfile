# =============================
# Step 1: Build frontend (Vue)
# =============================
FROM node:20 AS frontend
WORKDIR /frontend

# 複製前端專案
COPY project/ .

# 安裝依賴並打包
RUN npm install
RUN npm run build

# =============================
# Step 2: Build backend (ASP.NET Core)
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製整個 ASP.NET Core 專案到容器
COPY manufacturing_system/. ./manufacturing_system/

# Restore 依賴
RUN dotnet restore ./manufacturing_system/ManufacturingSystem.csproj

# 複製前端打包好的檔案到 wwwroot
COPY --from=frontend /frontend/dist ./manufacturing_system/wwwroot

# 發佈為 Release
RUN dotnet publish ./manufacturing_system/ManufacturingSystem.csproj -c Release -o /app/publish

# =============================
# Step 3: Runtime image
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 複製發佈好的檔案
COPY --from=build /app/publish ./

# 開放埠號
EXPOSE 8080

# 啟動 ASP.NET Core 應用
ENTRYPOINT ["dotnet", "ManufacturingSystem.dll"]
