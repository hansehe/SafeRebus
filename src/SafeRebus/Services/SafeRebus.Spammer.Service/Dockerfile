FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY . .
WORKDIR /src/SafeRebus/Services/SafeRebus.Spammer.Service
RUN dotnet publish SafeRebus.Spammer.Service.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SafeRebus.Spammer.Service.dll"]
