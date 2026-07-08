#!/usr/bin/env bash
# Wires ProjectReferences and baseline PackageReferences for every module,
# following the dependency rule from architecture section 06:
#   Presentation -> Application -> Domain ; Infrastructure -> Application + Domain
# No module ever references another module's assembly — cross-module
# communication happens exclusively through integration events (section 09).
set -euo pipefail
cd "$(dirname "$0")/.."

SK="src/Shared/SpeededERP.SharedKernel/SpeededERP.SharedKernel.csproj"
BB_DOMAIN="src/Shared/SpeededERP.BuildingBlocks.Domain/SpeededERP.BuildingBlocks.Domain.csproj"
BB_APP="src/Shared/SpeededERP.BuildingBlocks.Application/SpeededERP.BuildingBlocks.Application.csproj"
BB_INFRA="src/Shared/SpeededERP.BuildingBlocks.Infrastructure/SpeededERP.BuildingBlocks.Infrastructure.csproj"
BB_MSG="src/Shared/SpeededERP.BuildingBlocks.Messaging/SpeededERP.BuildingBlocks.Messaging.csproj"
BB_WEB="src/Shared/SpeededERP.BuildingBlocks.Web/SpeededERP.BuildingBlocks.Web.csproj"

echo "== SharedKernel & BuildingBlocks =="
dotnet add "$BB_DOMAIN" reference "$SK" >/dev/null
dotnet add "$BB_APP" reference "$SK" "$BB_DOMAIN" >/dev/null
dotnet add "$BB_APP" package MediatR >/dev/null
dotnet add "$BB_APP" package FluentValidation >/dev/null
dotnet add "$BB_APP" package FluentValidation.DependencyInjectionExtensions >/dev/null
dotnet add "$BB_APP" package Mapster >/dev/null
dotnet add "$BB_INFRA" reference "$SK" "$BB_DOMAIN" "$BB_APP" >/dev/null
dotnet add "$BB_INFRA" package Microsoft.EntityFrameworkCore >/dev/null
dotnet add "$BB_INFRA" package Microsoft.EntityFrameworkCore.Relational >/dev/null
dotnet add "$BB_INFRA" package StackExchange.Redis >/dev/null
dotnet add "$BB_MSG" reference "$SK" >/dev/null
dotnet add "$BB_MSG" package MassTransit >/dev/null
dotnet add "$BB_WEB" reference "$SK" "$BB_APP" >/dev/null
dotnet add "$BB_WEB" package Asp.Versioning.Http >/dev/null
dotnet add "$BB_WEB" package Asp.Versioning.Mvc.ApiExplorer >/dev/null
dotnet add "$SK" package Ardalis.GuardClauses >/dev/null

wire_module() {
  local base="$1" # e.g. src/Modules/Core/Identity/SpeededERP.Modules.Core.Identity
  local name="$2" # e.g. SpeededERP.Modules.Core.Identity

  local dom="$base.Domain/$name.Domain.csproj"
  local app="$base.Application/$name.Application.csproj"
  local infra="$base.Infrastructure/$name.Infrastructure.csproj"
  local pres="$base.Presentation/$name.Presentation.csproj"

  dotnet add "$dom" reference "$SK" >/dev/null

  dotnet add "$app" reference "$dom" "$BB_APP" >/dev/null

  dotnet add "$infra" reference "$app" "$dom" "$BB_INFRA" "$BB_MSG" >/dev/null
  dotnet add "$infra" package Microsoft.EntityFrameworkCore >/dev/null
  dotnet add "$infra" package Npgsql.EntityFrameworkCore.PostgreSQL >/dev/null
  dotnet add "$infra" package MassTransit.RabbitMQ >/dev/null
  dotnet add "$infra" package Microsoft.EntityFrameworkCore.Design >/dev/null

  dotnet add "$pres" reference "$infra" "$BB_WEB" >/dev/null
  dotnet add "$pres" package HotChocolate.AspNetCore >/dev/null
}

CORE_MODULES="Identity Metadata Workflow Collaboration Documents Tasks Config"
BUSINESS_MODULES="CRM Comptabilite RH Stock Logistique Immobilier"

echo "== Core modules =="
for M in $CORE_MODULES; do
  wire_module "src/Modules/Core/$M/SpeededERP.Modules.Core.$M" "SpeededERP.Modules.Core.$M"
done

echo "== Business modules =="
for M in $BUSINESS_MODULES; do
  wire_module "src/Modules/Business/$M/SpeededERP.Modules.Business.$M" "SpeededERP.Modules.Business.$M"
done

echo "== Host.Api =="
API="src/Host/SpeededERP.Api/SpeededERP.Api.csproj"
dotnet add "$API" reference "$BB_WEB" "$BB_APP" "$BB_INFRA" "$BB_MSG" >/dev/null
for M in $CORE_MODULES; do
  dotnet add "$API" reference "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.Presentation/SpeededERP.Modules.Core.$M.Presentation.csproj" >/dev/null
done
for M in $BUSINESS_MODULES; do
  dotnet add "$API" reference "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.Presentation/SpeededERP.Modules.Business.$M.Presentation.csproj" >/dev/null
done
dotnet add "$API" package HotChocolate.AspNetCore >/dev/null
dotnet add "$API" package NSwag.AspNetCore >/dev/null
dotnet add "$API" package Serilog.AspNetCore >/dev/null
dotnet add "$API" package Serilog.Sinks.Console >/dev/null
dotnet add "$API" package Serilog.Enrichers.Environment >/dev/null
dotnet add "$API" package OpenTelemetry.Extensions.Hosting >/dev/null
dotnet add "$API" package OpenTelemetry.Exporter.OpenTelemetryProtocol >/dev/null
dotnet add "$API" package OpenTelemetry.Instrumentation.AspNetCore >/dev/null
dotnet add "$API" package OpenTelemetry.Instrumentation.Http >/dev/null
dotnet add "$API" package OpenIddict.AspNetCore >/dev/null
dotnet add "$API" package Finbuckle.MultiTenant.AspNetCore >/dev/null
dotnet add "$API" package Finbuckle.MultiTenant.EntityFrameworkCore >/dev/null
dotnet add "$API" package Microsoft.FeatureManagement.AspNetCore >/dev/null
dotnet add "$API" package Asp.Versioning.Http >/dev/null
dotnet add "$API" package Asp.Versioning.Mvc.ApiExplorer >/dev/null
dotnet add "$API" package AspNetCore.HealthChecks.NpgSql >/dev/null
dotnet add "$API" package AspNetCore.HealthChecks.Redis >/dev/null
dotnet add "$API" package AspNetCore.HealthChecks.Rabbitmq >/dev/null

echo "== Host.Worker =="
WORKER="src/Host/SpeededERP.Worker/SpeededERP.Worker.csproj"
dotnet add "$WORKER" reference "$BB_INFRA" "$BB_MSG" >/dev/null
for M in $CORE_MODULES; do
  dotnet add "$WORKER" reference "src/Modules/Core/$M/SpeededERP.Modules.Core.$M.Infrastructure/SpeededERP.Modules.Core.$M.Infrastructure.csproj" >/dev/null
done
for M in $BUSINESS_MODULES; do
  dotnet add "$WORKER" reference "src/Modules/Business/$M/SpeededERP.Modules.Business.$M.Infrastructure/SpeededERP.Modules.Business.$M.Infrastructure.csproj" >/dev/null
done
dotnet add "$WORKER" package MassTransit.RabbitMQ >/dev/null
dotnet add "$WORKER" package Hangfire.AspNetCore >/dev/null
dotnet add "$WORKER" package Hangfire.PostgreSql >/dev/null
dotnet add "$WORKER" package Serilog.AspNetCore >/dev/null
dotnet add "$WORKER" package Serilog.Sinks.Console >/dev/null
dotnet add "$WORKER" package OpenTelemetry.Extensions.Hosting >/dev/null
dotnet add "$WORKER" package OpenTelemetry.Exporter.OpenTelemetryProtocol >/dev/null

echo "== Gateway =="
GW="src/Gateway/SpeededERP.Gateway/SpeededERP.Gateway.csproj"
dotnet add "$GW" package Yarp.ReverseProxy >/dev/null
dotnet add "$GW" package Serilog.AspNetCore >/dev/null

echo "Done."
