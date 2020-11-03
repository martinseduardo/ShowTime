FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
EXPOSE 5051

# Copiar csproj e restaurar dependencias
COPY Application/*.csproj ./
RUN dotnet restore

# Build da aplicacao
COPY Application/ ./
RUN dotnet publish -c Release -o out

# Build da imagem
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS http://*:5051
ENTRYPOINT ["dotnet", "Application.dll"]