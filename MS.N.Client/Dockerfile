FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY BGBA.Edge.Microservices.ConsultaCliente.sln ./
COPY BGBA.Edge.Microservices.ConsultaCliente/BGBA.Edge.Microservices.ConsultaCliente.csproj BGBA.Edge.Microservices.ConsultaCliente/
COPY BGBA.Edge.Core.Models/BGBA.Edge.Core.Models.csproj BGBA.Edge.Core.Models/
COPY BGBA.Edge.Services.ConsultaCliente/BGBA.Edge.Services.ConsultaCliente.csproj BGBA.Edge.Services.ConsultaCliente/
COPY BGBA.Edge.Core.Rest/BGBA.Edge.Core.Rest.csproj BGBA.Edge.Core.Rest/
COPY BGBA.Edge.Core.Trace/BGBA.Edge.Core.Trace.csproj BGBA.Edge.Core.Trace/
COPY BGBA.Edge.Models.Padron/BGBA.Edge.Models.Padron.csproj BGBA.Edge.Models.Padron/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/BGBA.Edge.Microservices.ConsultaCliente
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BGBA.Edge.Microservices.ConsultaCliente.dll"]
