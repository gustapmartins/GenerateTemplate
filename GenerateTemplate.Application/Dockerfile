# Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["GenerateTemplate.Application/GenerateTemplate.Application.csproj", "GenerateTemplate.Application/"]
COPY ["GenerateTemplate.CrossCutting/GenerateTemplate.Infra.CrossCutting.csproj", "GenerateTemplate.Infra.CrossCutting/"]
COPY ["GenerateTemplate.Domain/GenerateTemplate.Domain.csproj", "GenerateTemplate.Domain/"]
COPY ["GenerateTemplate.Data/GenerateTemplate.Infra.Data.csproj", "GenerateTemplate.Infra.Data/"]
COPY ["GenerateTemplate.Test/GenerateTemplate.Test.csproj", "GenerateTemplate.Test/"]

# Restaura as dependências do projeto
RUN dotnet restore "GenerateTemplate.Application/GenerateTemplate.Application.csproj"

COPY . .
WORKDIR "/src/GenerateTemplate.Application"
RUN dotnet build "GenerateTemplate.Application.csproj" -c Release -o /app/build 

# Esta fase é usada na produção ou quando executada no VS no modo normal
FROM build AS final
WORKDIR /app
COPY --from=build /app/build .
EXPOSE 80

ENTRYPOINT ["dotnet", "GenerateTemplate.Application.dll", "--urls", "http://+:443;http://*:80"]