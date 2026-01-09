#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["Server/Server.csproj", "Server/"]
#RUN dotnet restore "Server/Server.csproj"
#COPY . .
#WORKDIR "/src/Server"
#RUN dotnet build "Server.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Server.dll"]

# 階段 1：執行階段環境 (Base)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Cloud Run 預設只監聽 8080，8081 (HTTPS) 在 Cloud Run 內部不需要，由 GCP 處理
EXPOSE 8080

# 階段 2：SDK 編譯 (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 注意：如果 Dockerfile 在 Server 資料夾內，複製路徑要改
COPY ["Server.csproj", "./"]
RUN dotnet restore "Server.csproj"

COPY . .
RUN dotnet build "Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 階段 3：發佈 (Publish)
FROM build AS publish
RUN dotnet publish "Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 階段 4：最終運行鏡像 (Final)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 關鍵設定：強制 .NET 監聽 8080，否則 Cloud Run 會偵測不到服務
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Server.dll"]