FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY . .
WORKDIR /src/SafeRebus/Services/SafeRebus.OutboxCleaner.Service
RUN dotnet publish SafeRebus.OutboxCleaner.Service.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SafeRebus.OutboxCleaner.Service.dll"]
