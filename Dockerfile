# Step 1: Build frontend (Vue)
FROM node:20 AS frontend
WORKDIR /frontend
COPY project/ .
RUN npm install && npm run build

# Step 2: Build backend (ASP.NET Core)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 複製 ASP.NET Core 專案檔
COPY manufacturing-system-asp.net/ .

# 複製前端打包好的檔案到 wwwroot
COPY --from=frontend /frontend/dist ./wwwroot

# 還原並建置 (Release)
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Step 3: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 複製建置成果
COPY --from=build /app/publish .

# 開放應用程式埠
EXPOSE 8080

# 啟動 ASP.NET Core
ENTRYPOINT ["dotnet", "Manufacturing.Api.dll"]