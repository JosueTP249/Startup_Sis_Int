#Construccion (.NET V10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

#Archivos del proyecto y restaurar paquetes
COPY ["Frontedn_Posture AI.csproj", "./"]
RUN dotnet restore "Frontedn_Posture AI.csproj"

#Copiar el codigo fuente y compilar en Release
COPY . .
RUN dotnet publish "Frontedn_Posture AI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa de producción (.NET 10)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Dependencia nativa recomendada para ONNX Runtime en Linux
RUN apt-get update && apt-get install -y --no-install-recommends libgomp1 && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Puerto estándar Google Cloud Run
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Frontedn_Posture AI.dll"]