FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY . .
WORKDIR /src/SafeRebus/Services/SafeRebus.MessageSpammer.Service
RUN dotnet publish SafeRebus.MessageSpammer.Service.csproj -c Release -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SafeRebus.MessageSpammer.Service.dll"]
