# ========================================
# Stage 1: Restore & Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /src

# Copy everything (including submodules)
COPY . .

# Initialize and update git submodules only when this is actually a git checkout
RUN if [ -f .gitmodules ] && [ -d .git ]; then \
      git submodule init && git submodule update --recursive; \
    else \
      echo "No .git or .gitmodules found — skipping submodule init"; \
    fi

# Locate a .csproj file in the context, save its path, and restore
RUN set -eux; \
    proj=$(find . -type f -name '*.csproj' | head -n 1 || true); \
    if [ -z "$proj" ]; then \
      echo "ERROR: no .csproj file found in build context."; \
      echo "Repository tree:"; ls -la; echo; ls -R . | sed -n '1,200p'; \
      exit 1; \
    fi; \
    echo "Using project: $proj"; \
    echo "$proj" > /tmp/PROJECT_PATH; \
    dotnet restore "$proj"

# Build the project in Release mode
RUN dotnet build "$(cat /tmp/PROJECT_PATH)" -c Release -o /app/build

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish
RUN dotnet publish "$(cat /tmp/PROJECT_PATH)" -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime Image
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=publish /app/publish .

# Expose port (adjust as needed)
EXPOSE 8080

# Set entrypoint
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]