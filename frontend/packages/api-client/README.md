# @speedederp/api-client

Two layers, deliberately kept separate:

1. **Hand-written client (what exists today)** — `src/httpClient.ts`,
   `src/auth.ts`, `src/identity.ts`, `src/metadata.ts`, `src/crm.ts`, composed by
   `src/SpeededErpApiClient.ts`. Typed against the REST endpoints that are actually
   implemented in the backend right now (see the `Endpoints/` folder of each
   module's Presentation project). This is what apps import.

2. **OpenAPI codegen (for later)** — once the backend is running locally
   (`dotnet run --project src/Host/SpeededERP.Api`), its NSwag-generated OpenAPI
   document is served at `/swagger/v1/swagger.json`. Run:

   ```bash
   pnpm --filter @speedederp/api-client generate:api-types
   ```

   This writes `src/generated/openapi.d.ts` — full types for every endpoint,
   including ones this hand-written client doesn't cover yet. As the backend
   surface grows, prefer migrating call sites to the generated types over hand
   -maintaining more interfaces here; the hand-written client's job is to stay a
   thin, dependency-free layer, not to duplicate a full OpenAPI client generator.

## Why not fully generated from day one?

The generated file doesn't exist in this repo (it requires a live backend to
produce), and shipping a hand-written client against the known, stable subset of
endpoints is more useful for the frontend apps to build against today than shipping
nothing until CI can run the backend and regenerate types.
