FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Core.N.Rest.sln ./
COPY ../workarea/GitOrganizacion/MS.N.Location/MS.N.Location.csproj ../workarea/GitOrganizacion/MS.N.Location/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/../workarea/GitOrganizacion/MS.N.Location
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MS.N.Location.dll"]
