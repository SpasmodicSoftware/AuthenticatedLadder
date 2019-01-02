FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["GenericAuthenticatedLadder/GenericAuthenticatedLadder.csproj", "GenericAuthenticatedLadder/"]
RUN dotnet restore "GenericAuthenticatedLadder/GenericAuthenticatedLadder.csproj"
COPY . .
WORKDIR "/src/GenericAuthenticatedLadder"
RUN dotnet build "GenericAuthenticatedLadder.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GenericAuthenticatedLadder.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GenericAuthenticatedLadder.dll"]