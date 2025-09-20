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

# 複製 .csproj 並還原依賴
COPY manufacturing_system/*.csproj ./manufacturing_system/
RUN dotnet restore ./manufacturing_system/manufacturing_system.csproj

# 複製整個 ASP.NET Core 專案
COPY manufacturing_system/. ./manufacturing_system/

# 複製前端打包好的檔案到 wwwroot
COPY --from=frontend /frontend/dist ./manufacturing_system/wwwroot

# 發佈為 self-contained 應用（Release 模式）
RUN dotnet publish ./manufacturing_system/manufacturing_system.csproj -c Release -o /app/publish

# =============================
# Step 3: Runtime image
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 複製發佈好的檔案
COPY --from=build /app/publish ./

# 開放應用程式埠
EXPOSE 8080

# 啟動 ASP.NET Core 應用
ENTRYPOINT ["dotnet", "manufacturing_system.dll"]
