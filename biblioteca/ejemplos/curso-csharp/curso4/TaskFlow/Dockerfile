# ---------- Etapa 1: compilar y publicar (imagen grande con el SDK) ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Primero solo los .csproj: así Docker reutiliza la capa de "restore" mientras no cambien las dependencias
COPY src/TaskFlow.Core/TaskFlow.Core.csproj                       src/TaskFlow.Core/
COPY src/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj   src/TaskFlow.Infrastructure/
COPY src/TaskFlow.Api/TaskFlow.Api.csproj                         src/TaskFlow.Api/
RUN dotnet restore src/TaskFlow.Api/TaskFlow.Api.csproj
COPY src/ src/
RUN dotnet publish src/TaskFlow.Api/TaskFlow.Api.csproj -c Release -o /app --no-restore

# ---------- Etapa 2: imagen final pequeña (solo el runtime de ASP.NET) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
# No ejecutar como root: las imágenes oficiales traen el usuario "app"
USER app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskFlow.Api.dll"]
