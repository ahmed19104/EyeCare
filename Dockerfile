# 1. Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# 2. Build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first to optimize Docker layer caching (تم تعديل اسم فولدر الـ API إلى EyeCare)
COPY ["EyeCare/EyeCareAPI.csproj", "EyeCare/"]
COPY ["EyeCare.Application/EyeCare.Application.csproj", "EyeCare.Application/"]
COPY ["EyeCare.Infrastructure/EyeCare.Infrastructure.csproj", "EyeCare.Infrastructure/"]
COPY ["EyeCare.Domain/EyeCare.Domain.csproj", "EyeCare.Domain/"]

# Restore dependencies
RUN dotnet restore "EyeCare/EyeCareAPI.csproj"

# Copy the remaining source code
COPY . .

# Build and publish
WORKDIR /src/EyeCare
RUN dotnet publish "EyeCareAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Final runtime image
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EyeCareAPI.dll"]
