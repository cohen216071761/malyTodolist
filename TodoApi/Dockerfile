# Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

# העתק קובץ הפרויקט
COPY TodoApi.csproj ./
RUN dotnet restore TodoApi.csproj

# העתק שאר הקבצים
COPY . ./

# פרסום באופציית Release
RUN dotnet publish TodoApi.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# העתק מהשלב הקודם
COPY --from=build-backend /app/publish .

# מאזין לפורט שספק Render נותן
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "TodoApi.dll"]
