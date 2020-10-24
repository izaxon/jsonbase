FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["jsonbase.csproj", "./"]
RUN dotnet restore "jsonbase.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "jsonbase.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "jsonbase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ROOT_PATH=./data
ENTRYPOINT ["dotnet", "jsonbase.dll"]
