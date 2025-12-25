# 1. Aşama: Çalışma Ortamı (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. Aşama: Derleme (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Proje dosyasını kopyalıyoruz
COPY ["SAT242516060.csproj", "."]
RUN dotnet restore "./SAT242516060.csproj"

# Tüm kodları kopyalayıp derleme işlemini yapıyoruz
COPY . .
WORKDIR "/src/."
RUN dotnet build "SAT242516060.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. Aşama: Yayınlama (Publish)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SAT242516060.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. Aşama: Final (Konteyner Başlatma)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SAT242516060.dll"]