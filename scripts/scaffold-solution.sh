#!/usr/bin/env bash
# Scaffolds every .csproj of the SpeededERP modular monolith and wires it into the .sln.
# Idempotent: skips a project if its .csproj already exists.
set -euo pipefail
cd "$(dirname "$0")/.."
ROOT="$(pwd)"

strip_tfm() {
  # Directory.Build.props already pins net8.0 for every project; remove the
  # per-project <TargetFramework> emitted by the template so it doesn't collide.
  sed -i '/<TargetFramework>/d' "$1"
}

new_lib() {
  local path="$1" name="$2"
  if [ ! -f "$path/$name.csproj" ]; then
    dotnet new classlib -n "$name" -o "$path" --no-restore >/dev/null
    rm -f "$path/Class1.cs"
    strip_tfm "$path/$name.csproj"
  fi
  dotnet sln add "$path/$name.csproj" >/dev/null
}

echo "== Shared =="
new_lib "src/Shared/SpeededERP.SharedKernel" "SpeededERP.SharedKernel"
for L in Domain Application Infrastructure Messaging Web; do
  new_lib "src/Shared/SpeededERP.BuildingBlocks.$L" "SpeededERP.BuildingBlocks.$L"
done

CORE_MODULES="Identity Metadata Workflow Collaboration Documents Tasks Config"
BUSINESS_MODULES="CRM Comptabilite RH Stock Logistique Immobilier"

echo "== Core modules =="
for M in $CORE_MODULES; do
  for L in Domain Application Infrastructure Presentation; do
    new_lib "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.$L" "SpeededERP.Modules.Core.$M.$L"
  done
done

echo "== Business modules =="
for M in $BUSINESS_MODULES; do
  for L in Domain Application Infrastructure Presentation; do
    new_lib "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.$L" "SpeededERP.Modules.Business.$M.$L"
  done
done

echo "== Host =="
if [ ! -f "src/Host/SpeededERP.Api/SpeededERP.Api.csproj" ]; then
  dotnet new web -n "SpeededERP.Api" -o "src/Host/SpeededERP.Api" --no-restore >/dev/null
  strip_tfm "src/Host/SpeededERP.Api/SpeededERP.Api.csproj"
fi
dotnet sln add "src/Host/SpeededERP.Api/SpeededERP.Api.csproj" >/dev/null

if [ ! -f "src/Host/SpeededERP.Worker/SpeededERP.Worker.csproj" ]; then
  dotnet new worker -n "SpeededERP.Worker" -o "src/Host/SpeededERP.Worker" --no-restore >/dev/null
  strip_tfm "src/Host/SpeededERP.Worker/SpeededERP.Worker.csproj"
fi
dotnet sln add "src/Host/SpeededERP.Worker/SpeededERP.Worker.csproj" >/dev/null

echo "== Gateway =="
if [ ! -f "src/Gateway/SpeededERP.Gateway/SpeededERP.Gateway.csproj" ]; then
  dotnet new web -n "SpeededERP.Gateway" -o "src/Gateway/SpeededERP.Gateway" --no-restore >/dev/null
  strip_tfm "src/Gateway/SpeededERP.Gateway/SpeededERP.Gateway.csproj"
fi
dotnet sln add "src/Gateway/SpeededERP.Gateway/SpeededERP.Gateway.csproj" >/dev/null

echo "== Tests =="
new_lib "tests/ArchitectureTests/SpeededERP.ArchitectureTests" "SpeededERP.ArchitectureTests"

for M in $CORE_MODULES; do
  new_lib "tests/UnitTests/Core.$M.UnitTests" "SpeededERP.Modules.Core.$M.UnitTests"
  new_lib "tests/IntegrationTests/Core.$M.IntegrationTests" "SpeededERP.Modules.Core.$M.IntegrationTests"
done
for M in $BUSINESS_MODULES; do
  new_lib "tests/UnitTests/Business.$M.UnitTests" "SpeededERP.Modules.Business.$M.UnitTests"
  new_lib "tests/IntegrationTests/Business.$M.IntegrationTests" "SpeededERP.Modules.Business.$M.IntegrationTests"
done

echo "Done."
