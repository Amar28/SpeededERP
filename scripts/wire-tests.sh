#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

add_test_pkgs() {
  local proj="$1"
  dotnet add "$proj" package Microsoft.NET.Test.Sdk >/dev/null
  dotnet add "$proj" package xunit >/dev/null
  dotnet add "$proj" package xunit.runner.visualstudio >/dev/null
  dotnet add "$proj" package FluentAssertions >/dev/null
  dotnet add "$proj" package NSubstitute >/dev/null
}

CORE_MODULES="Identity Metadata Workflow Collaboration Documents Tasks Config"
BUSINESS_MODULES="CRM Comptabilite RH Stock Logistique Immobilier"

echo "== Unit tests =="
for M in $CORE_MODULES; do
  base="tests/UnitTests/Core.$M.UnitTests/SpeededERP.Modules.Core.$M.UnitTests.csproj"
  add_test_pkgs "$base"
  dotnet add "$base" reference \
    "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.Domain/SpeededERP.Modules.Core.$M.Domain.csproj" \
    "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.Application/SpeededERP.Modules.Core.$M.Application.csproj" >/dev/null
done
for M in $BUSINESS_MODULES; do
  base="tests/UnitTests/Business.$M.UnitTests/SpeededERP.Modules.Business.$M.UnitTests.csproj"
  add_test_pkgs "$base"
  dotnet add "$base" reference \
    "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.Domain/SpeededERP.Modules.Business.$M.Domain.csproj" \
    "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.Application/SpeededERP.Modules.Business.$M.Application.csproj" >/dev/null
done

echo "== Integration tests =="
for M in $CORE_MODULES; do
  base="tests/IntegrationTests/Core.$M.IntegrationTests/SpeededERP.Modules.Core.$M.IntegrationTests.csproj"
  add_test_pkgs "$base"
  dotnet add "$base" package Testcontainers.PostgreSql >/dev/null
  dotnet add "$base" package Respawn >/dev/null
  dotnet add "$base" reference \
    "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.Infrastructure/SpeededERP.Modules.Core.$M.Infrastructure.csproj" \
    "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.Presentation/SpeededERP.Modules.Core.$M.Presentation.csproj" >/dev/null
done
for M in $BUSINESS_MODULES; do
  base="tests/IntegrationTests/Business.$M.IntegrationTests/SpeededERP.Modules.Business.$M.IntegrationTests.csproj"
  add_test_pkgs "$base"
  dotnet add "$base" package Testcontainers.PostgreSql >/dev/null
  dotnet add "$base" package Respawn >/dev/null
  dotnet add "$base" reference \
    "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.Infrastructure/SpeededERP.Modules.Business.$M.Infrastructure.csproj" \
    "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.Presentation/SpeededERP.Modules.Business.$M.Presentation.csproj" >/dev/null
done

echo "== Architecture tests =="
ARCH="tests/ArchitectureTests/SpeededERP.ArchitectureTests/SpeededERP.ArchitectureTests.csproj"
add_test_pkgs "$ARCH"
dotnet add "$ARCH" package NetArchTest.Rules >/dev/null
dotnet add "$ARCH" reference src/Host/SpeededERP.Api/SpeededERP.Api.csproj >/dev/null
for M in $CORE_MODULES; do
  for L in Domain Application Infrastructure Presentation; do
    dotnet add "$ARCH" reference "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.$L/SpeededERP.Modules.Core.$M.$L.csproj" >/dev/null
  done
done
for M in $BUSINESS_MODULES; do
  for L in Domain Application Infrastructure Presentation; do
    dotnet add "$ARCH" reference "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.$L/SpeededERP.Modules.Business.$M.$L.csproj" >/dev/null
  done
done

echo "Done."
