FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["AuthenticatedLadder/AuthenticatedLadder.csproj", "AuthenticatedLadder/"]
RUN dotnet restore "AuthenticatedLadder/AuthenticatedLadder.csproj"
COPY . .
WORKDIR "/src/AuthenticatedLadder"
RUN dotnet build "AuthenticatedLadder.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AuthenticatedLadder.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthenticatedLadder.dll"]