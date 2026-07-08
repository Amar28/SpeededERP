# SpeededERP

SpeededERP est une plateforme ERP modulaire, personnalisable sans code, construite en
**modular monolith .NET 8** (Clean Architecture / DDD / CQRS / Event-Driven) avec un
frontend **React 18 + TypeScript** en monorepo pnpm.

Ce dépôt est un **squelette complet et buildable** : la solution .NET (90 projets) compile
sans erreur, les 5 images Docker se construisent et démarrent, les manifests Kubernetes se
rendent sans erreur, et le frontend passe typecheck/build/test/lint. Trois modules sont
implémentés en profondeur comme référence (voir tableau ci-dessous) ; les dix autres sont
scaffoldés avec un agrégat réel, CQRS, persistence EF Core et endpoints REST, prêts à être
étoffés en suivant les mêmes conventions.

Pour le détail architectural complet (couches, règles de dépendance, écarts assumés par
rapport au document d'architecture source, pistes de continuation), voir
[ARCHITECTURE.md](ARCHITECTURE.md).

## Stack technique

**Backend** — .NET 8, ASP.NET Core Minimal APIs + HotChocolate GraphQL, MediatR (CQRS),
FluentValidation, EF Core 8 + Npgsql, MassTransit + RabbitMQ, OpenIddict (OAuth2/OIDC),
ASP.NET Core Identity, Redis, Elasticsearch, Hangfire, Serilog, OpenTelemetry.

**Frontend** — React 18, TypeScript, MUI v6, Vite, pnpm workspaces, React Query,
React Hook Form + Zod, React Router 7, i18next, Playwright (e2e), Vitest.

**Infrastructure** — Docker multi-stage builds, Kubernetes + Kustomize
(overlays dev / staging / prod / saas / onprem), YARP (gateway), GitHub Actions CI,
Dependabot.

## Structure du dépôt

```
src/
  Shared/                    SharedKernel, BuildingBlocks.Application, .Infrastructure, .Web
  Modules/
    Core/                    Identity, Metadata, Workflow, Collaboration, Documents, Tasks, Config
    Business/                CRM, Comptabilite, RH, Stock, Logistique, Immobilier
  Host/                      SpeededERP.Api, SpeededERP.Worker
  Gateway/                   SpeededERP.Gateway (YARP)
tests/
  UnitTests/, IntegrationTests/, ArchitectureTests/
frontend/
  packages/                  core, ui-kit, form-engine, api-client
  apps/                      admin-portal, tenant-portal
  e2e/                       Playwright
deploy/
  docker/                    docker-compose.yml, Dockerfile.*, nginx-spa.conf, .env.example
  k8s/                       base/ + overlays/{dev,staging,prod,saas,onprem}
.github/workflows/ci.yml, .github/dependabot.yml
scripts/                     scaffold-solution.sh, wire-references.sh, wire-tests.sh
ARCHITECTURE.md              document "as-built" détaillé
```

## Statut des modules

| Module | Domaine | Application | Infrastructure | Presentation | Profondeur |
|---|---|---|---|---|---|
| Core.Identity | ✅ | ✅ | ✅ | ✅ | Référence — OpenIddict, ASP.NET Identity, tenants, rôles/permissions |
| Core.Metadata | ✅ | ✅ | ✅ | ✅ | Référence — EntityDefinition/FieldDefinition/FormDefinition, moteur de règles JsonLogic |
| Business.CRM | ✅ | ✅ | ✅ | ✅ | Référence — Devis/Client, pattern Outbox canonique (`AcceptDevisCommandHandler`) |
| Core.Workflow | ✅ | ✅ | ✅ | ✅ | Scaffoldé — 1 agrégat, CQRS, EF Core, REST |
| Core.Collaboration | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Core.Documents | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Core.Tasks | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Core.Config | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Business.Comptabilite | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Business.RH | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Business.Stock | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Business.Logistique | ✅ | ✅ | ✅ | ✅ | Scaffoldé |
| Business.Immobilier | ✅ | ✅ | ✅ | ✅ | Scaffoldé |

Les modules "Scaffoldé" suivent exactement les conventions des trois modules de
référence (mêmes couches, mêmes `Behaviors`, même pattern Outbox, mêmes règles
d'architecture vérifiées par `SpeededERP.ArchitectureTests`) : les étoffer consiste à
ajouter des agrégats, commandes/requêtes et endpoints supplémentaires sans rien
réinventer.

## Démarrage rapide

### Prérequis

- .NET SDK 8.0.1xx (voir [global.json](global.json))
- Node.js 22+ et pnpm (`corepack enable`)
- Docker Desktop (pour l'environnement complet via docker-compose)

### Option A — Stack complète via Docker Compose

```bash
cd deploy/docker
cp .env.example .env    # ajuster les mots de passe si besoin
docker compose --env-file .env up --build
```

Services exposés :

| Service | URL |
|---|---|
| API (`SpeededERP.Api`) | http://localhost:8080 |
| Gateway YARP | http://localhost:8000 |
| Admin Portal | http://localhost:5173 |
| Tenant Portal | http://localhost:5174 |
| RabbitMQ management | http://localhost:15672 |
| PostgreSQL | localhost:5432 |
| Redis | localhost:6379 |
| Elasticsearch | http://localhost:9200 |

### Option B — Développement local (sans conteneurs pour l'app)

Backend :

```bash
dotnet restore
dotnet build
dotnet run --project src/Host/SpeededERP.Api
dotnet run --project src/Host/SpeededERP.Worker
```

Frontend :

```bash
cd frontend
pnpm install
pnpm -r build      # build initial des packages partagés (core, ui-kit, form-engine, api-client)
pnpm --filter admin-portal dev    # http://localhost:5173
pnpm --filter tenant-portal dev   # http://localhost:5174
```

Les apps front s'attendent à trouver l'API/gateway sur `http://localhost:8000`
(`VITE_API_BASE_URL`, voir `frontend/apps/*/vite.config.ts`).

### Tests

```bash
# Backend — unitaires, intégration, règles d'architecture (NetArchTest)
dotnet test

# Frontend
cd frontend
pnpm typecheck
pnpm -r test
pnpm -r lint
pnpm --filter admin-portal e2e     # Playwright, nécessite la stack Docker démarrée
```

### Kubernetes

```bash
kubectl kustomize deploy/k8s/overlays/dev
kubectl apply -k deploy/k8s/overlays/dev
```

Overlays disponibles : `dev`, `staging`, `prod`, `saas`, `onprem` (voir
[ARCHITECTURE.md](ARCHITECTURE.md) pour la différence entre modes d'isolation multi-tenant).

## Bonnes pratiques appliquées

- **Clean Architecture / DDD** : chaque module expose Domain → Application → Infrastructure →
  Presentation, avec des règles de dépendance vérifiées automatiquement par
  `tests/ArchitectureTests` (NetArchTest) — le Domain n'a aucune dépendance externe.
- **CQRS** via MediatR, avec pipeline `Behaviors` (validation, logging, autorisation, cache,
  gestion d'exceptions) partagé par tous les modules.
- **Event-Driven** : Domain Events (in-process, post-commit) séparés des Integration Events
  (MassTransit/RabbitMQ, publiés via le pattern Outbox pour garantir l'atomicité avec l'écriture
  métier — voir `AcceptDevisCommandHandler` comme référence).
- **No-code** : le moteur de règles (`Core.Metadata`) est déclaratif (JsonLogic) — aucune
  exécution de code arbitraire côté serveur.
- **Sécurité** : OpenIddict avec clés éphémères en conteneur (pas de certificat persistant dans
  le home directory), permissions par rôle/tenant, CORS explicite, secrets jamais commités
  (voir `deploy/k8s/base/secret.yaml` — template uniquement).
- **Observabilité** : Serilog + OpenTelemetry (traces/métriques), health checks par service.
- **Documentation** : OpenAPI généré par l'API, ce README, [ARCHITECTURE.md](ARCHITECTURE.md).
- **CI/CD** : [.github/workflows/ci.yml](.github/workflows/ci.yml) (build/test backend et
  frontend, build des images Docker, validation des manifests k8s) et
  [Dependabot](.github/dependabot.yml) (NuGet, npm, Docker, GitHub Actions).

## Documentation complémentaire

- [ARCHITECTURE.md](ARCHITECTURE.md) — document "as-built" : périmètre exact, écarts
  assumés par rapport à l'architecture source, carte des fichiers, règles de dépendance,
  pistes de continuation.
