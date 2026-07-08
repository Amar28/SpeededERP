# SpeededERP — Notes d'implémentation

Ce document complète le dossier d'architecture (v0.2) — il n'en est pas une copie. Il
explique ce qui a été **effectivement construit** dans ce dépôt, où trouver chaque
pièce, et les quelques décisions d'implémentation prises pour combler les détails que
le dossier d'architecture laisse volontairement ouverts.

> Dossier d'architecture source : voir le lien Artifact fourni en tête de projet
> (32 sections, Parties I à V). Ce fichier y fait référence par numéro de section.

## 1. Portée de ce premier passage

Les 13 modules (7 Core + 6 métier) existent tous avec leurs 4 couches hexagonales,
compilent avec **0 erreur** sur l'ensemble de la solution (90 projets), et respectent
les règles de dépendance de la section 06 — vérifié automatiquement par
`tests/ArchitectureTests` (65 tests, NetArchTest).

Trois modules sont implémentés **en profondeur**, avec une richesse fonctionnelle
représentative d'un module de production, et servent de gabarit pour les autres :

| Module | Ce qu'il démontre |
|---|---|
| **Core.Identity** | Agrégats `User`/`Role`/`Tenant`, séparation identité métier / identifiants techniques (ASP.NET Identity), OpenIddict (Authorization Code + PKCE, password grant pour outillage), RBAC à permissions atomiques |
| **Core.Metadata** | Le moteur no-code complet (section 10/11/13) : `EntityDefinition`/`FieldDefinition`/`FormDefinition`, `FieldValueValidator`, moteur de règles JsonLogic (`JsonLogicRuleEvaluator`), déclaration d'entités par événement d'intégration |
| **Business.CRM** | L'exemple fil rouge du dossier : agrégat `Devis` (section 07/09, pattern Outbox), agrégat `Client` (section 10, champs natifs + `custom_fields JSONB`) |

Les 10 autres modules sont **scaffoldés au même standard architectural** (agrégat
réel avec invariants, CQRS complet pour au moins un cas d'usage, EF Core, endpoints
REST, `ModuleRegistration`) mais avec une couverture fonctionnelle volontairement
réduite à 1-2 cas d'usage représentatifs plutôt que la liste complète de la section 05.
C'est un point de départ solide pour une équipe, pas un ERP fini.

## 2. Ce qui diffère du dossier d'architecture, et pourquoi

- **Cible .NET** : le dossier vise .NET 8 (LTS). L'environnement de génération
  n'avait que le SDK .NET 10 installé ; les projets ciblent **`net8.0`** (le SDK 10
  compile correctement des projets `net8.0` — les *ref assemblies* sont restaurées via
  NuGet), donc le code reste conforme à la cible documentée. Seuls certains packages
  (OpenTelemetry, `Microsoft.Extensions.*`) sont épinglés sur des versions plus
  récentes que le strict minimum net8, parce que leurs dernières releases net8-only
  contenaient une CVE modérée corrigée seulement dans des versions qui exigent
  `Microsoft.Extensions.*` ≥ 9.0.0 (paquets d'abstraction, compatibles net8 en
  pratique). Voir `Directory.Packages.props`.
- **Finbuckle.MultiTenant** : le dossier le nomme comme bibliothèque de résolution de
  tenant. Ce dépôt implémente la résolution de tenant "Shared" via un
  `ITenantContext` maison lisant le claim JWT `tenant_id` (voir
  `BuildingBlocks.Web.Security.HttpTenantContext`) — suffisant et testé pour le mode
  Shared. Le paquet Finbuckle reste **prévu mais non câblé** : c'est le point
  d'extension pour le mode "Dedicated" (routage vers une base par tenant), qui
  demande un store EF Core par-dessus `Core.Identity.Tenant` — décision reportée pour
  rester dans le temps imparti plutôt que livrer une intégration partielle.
- **Elsa Workflows** : le paquet est référencé dans `Directory.Packages.props` mais
  `Core.Workflow` n'héberge pas encore le moteur Elsa lui-même — il expose aujourd'hui
  son propre agrégat `WorkflowDefinition` (JSON validé, activable) comme point
  d'ancrage. Brancher Elsa en tant que moteur d'exécution est le prochain incrément
  naturel de ce module.
- **Domain Events vs Outbox — ordre d'exécution** : le dossier (Fig. 03) décrit
  l'écriture de l'agrégat et de l'événement d'Outbox dans la même transaction. Pour
  garantir cette atomicité sans complexité d'interception EF Core fragile, la règle
  retenue ici est explicite : **le command handler enqueue lui-même l'IntegrationEvent
  via `IOutboxWriter` avant d'appeler `SaveChangesAsync`** (voir
  `AcceptDevisCommandHandler` comme référence). Les Domain Events "purs" (MediatR) sont
  dispatchés *après* le commit (`DomainEventsInterceptor.SavedChangesAsync`) et ne
  servent qu'aux réactions in-process, non transactionnelles, au sein du même module —
  jamais à publier un IntegrationEvent (voir le commentaire dans
  `DomainEventsInterceptor.cs`).
- **Custom fields cross-module** : pour valider un `custom_fields JSONB` sans que
  chaque module métier référence l'assembly de Core.Metadata (interdit par la section
  06), le port `ICustomFieldsValidator` vit dans `BuildingBlocks.Application` ; seule
  son implémentation concrète est dans `Core.Metadata.Infrastructure`, injectée par
  DI au niveau du composition root. Voir `RegisterClientCommandHandler` dans CRM pour
  l'usage.

## 3. Où trouver quoi

```
src/Shared/                  SharedKernel + 5 BuildingBlocks (section 06)
src/Modules/Core/<Module>/   7 modules Core × 4 couches
src/Modules/Business/<Module>/ 6 modules métier × 4 couches
src/Host/SpeededERP.Api/     Composition root REST+GraphQL (ModuleCatalog.cs charge les 13 IModule)
src/Host/SpeededERP.Worker/  MassTransit consumers + Hangfire (WorkerModules.cs)
src/Gateway/SpeededERP.Gateway/ YARP — point de bascule microservices (section 32)
tests/UnitTests/             Domain/Application, par module
tests/IntegrationTests/      Harnais Testcontainers câblé, à remplir module par module
tests/ArchitectureTests/     Règles de dépendance section 06, automatisées
frontend/                    Monorepo pnpm — apps/ (admin-portal, tenant-portal), packages/ (ui-kit, form-engine, api-client, core)
deploy/docker/                docker-compose + Dockerfiles multi-stage
deploy/k8s/                   base + overlays (dev, staging, prod, saas, onprem)
```

## 4. Modèle de dépendances (rappel exécutable)

`Presentation → Infrastructure → Application → Domain`, `Domain` sans dépendance
externe (hors `Ardalis.GuardClauses`), aucun module ne référence l'assembly d'un
autre module — uniquement des contrats d'événements dans
`BuildingBlocks.Messaging.Contracts`. Ces règles ne sont pas que documentées : elles
sont vérifiées à chaque run de `tests/ArchitectureTests`.

## 5. Prochaines étapes suggérées

1. Brancher Elsa dans `Core.Workflow.Infrastructure` comme moteur d'exécution réel.
2. Ajouter le store EF Core Finbuckle pour le mode d'isolation "Dedicated".
3. Écrire les premiers tests d'intégration Testcontainers (le harnais existe déjà,
   `tests/IntegrationTests/<Module>` — voir `DevisRepository`/`CrmDbContext` pour le
   contexte à instancier).
4. Étoffer les modules scaffoldés (Comptabilite, RH, Stock, Logistique, Immobilier,
   Documents, Tasks, Config, Collaboration, Workflow) avec les cas d'usage restants de
   la section 05.
5. Générer le client TypeScript depuis l'OpenAPI réel une fois l'API lancée
   (`frontend/packages/api-client` documente la commande).
