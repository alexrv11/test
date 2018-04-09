FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Services.N.Core.Rest.sln ./
COPY MS.N.Afip/MS.N.Afip.csproj MS.N.Afip/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/MS.N.Afip
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MS.N.Afip.dll"]
