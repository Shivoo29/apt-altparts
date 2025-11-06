
# WHAT_TO_DO.md — APT Alternate Parts Monitoring 

> **Purpose:** End‑to‑end checklist to build, validate, release, and hand over the **Alternate Parts Monitoring** system (Shortages, PRs, Deviations) — replacing Excel with a controlled, auditable, full‑code solution.
>
> **Scope:** Full code (no Power BI/Power Automate). Tech stack: **.NET 8 (API & workers), EF Core 9, SQL Server, React + TypeScript**. Supports **file-based ingestion** first, with optional **Playwright exporter** (if approved) for automated CSV downloads.

---

## 0) Outcomes & Non‑Negotiables

- **Outcomes**
  - Single source of truth for Parts, Problem Reports (PRs), Shortages, and Deviations.
  - ≥50% manual effort reduction; ~300 hours/year saved.
  - Immutable **status history** for every PR (who/when/what/comments).
  - Validated **status transitions**; restricted edits via API only.
  - Read‑only Excel/CSV export for legacy consumers.

- **Non‑Negotiables / Constraints**
  - No direct ERP/iPLM API access in v1.
  - Prefer **file-based export** from source systems. Optional **UI automation exporter** only if approved.
  - LAM‑compliant: RBAC, audit logs, environments (Dev/Test/Prod), UAT evidence, change control.

- **Definition of Done (global)**
  - Code in main with passing build, unit tests, and migrations.
  - Security review notes, data flow diagram, and Work Instruction (WI) updated.
  - UAT sign‑off captured; release notes created; monitoring in place.

---

## 1) Governance & Compliance (LAM)

- [ ] Identify business owner, technical owner, and data steward.
- [ ] Document data classification (likely **Internal**; confirm any PII/PHI = expected **none**).
- [ ] Record **data flow diagram** (source → ingest → DB → API → UI → export).
- [ ] Define environments: Dev, Test (UAT), Prod; define promotion rules.
- [ ] Prepare **Security & Compliance notes**: RBAC model, audit history, backups/DR.
- [ ] Capture UAT evidence expectations (screenshots, logs, test cases).
- [ ] Draft and store **Work Instruction (WI)** outline in `/docs/WI.md`.

**Deliverables:** `/docs/security.md`, `/docs/data-flow.png`, `/docs/WI.md`, `/docs/release-checklist.md`

---

## 2) Data Sourcing & Ingestion Plan

- [ ] Confirm **CSV/XLSX export** availability from ERP/PLM/Analytics (fields, frequency, location).
- [ ] Define **drop zone** (local share/SharePoint/Blob) → Worker watch path.
- [ ] Lock **field mapping** and **ReasonCode whitelist** (focus: `ALT_PART`).
- [ ] Decide **dedupe keys**: `Parts` by `PartNumber`; `PRs` by `PR_SourceId` (or hash PartNumber+ReasonCode+OpenedDate).
- [ ] Define **rejects handling**: invalid rows → `ingest/rejects/yyyymmdd.csv` + log.

**Acceptance Criteria:**
- A sample export ingests end‑to‑end with 0 schema errors; duplicates are upserts, not inserts; rejects file is created for bad rows.

---

## 3) Database Schema & Migrations (EF Core)

- [ ] Implement entities (separate files) under `APT.Domain/Entities`:
  - `Part`, `ProblemReport`, `PRStatusHistory`, `Shortage`, `Deviation`.
- [ ] Configure `AppDbContext` relationships, indexes, and constraints.
- [ ] Create and apply **InitialCreate** EF migration.
- [ ] Add seed or smoke data script (optional).
- [ ] Add **bootstrap SQL** script in `ops/sql/bootstrap.sql` for environments without EF tools.

**Acceptance Criteria:**
- Database created; all tables present with correct FKs and indexes; `dotnet-ef database update` works from API project.

---

## 4) API (ASP.NET Core) — REST Endpoints

- [ ] Project scaffolding in `backend/src/APT.Api` with Swagger & CORS.
- [ ] Implement **ProblemReports** endpoints:
  - `GET /api/prs?reasonCode&status&skip&take` (server‑side paging, filters)
  - `GET /api/prs/{id}` (details + history)
  - `PATCH /api/prs/{id}/status` (validated transitions + append to `PRStatusHistory`)
- [ ] Implement **Parts** read endpoints (search by PartNumber, Plant).
- [ ] Implement **Shortages** read endpoints (optional for MVP if data is available).
- [ ] Implement **Deviations** create/read endpoints (simple approval flow for MVP).
- [ ] Centralized error handling + validation responses.
- [ ] Logging: request/response minimal logs, correlation id.

**Acceptance Criteria:**
- Swagger open; endpoints return expected DTOs; transitions blocked if invalid; history is written on each change.

---

## 5) Ingestion Worker (File → DB)

- [ ] Implement `APT.IngestWorker` with folder watcher / polling.
- [ ] Add parsers: **CsvHelper** for CSV; (optional) **ClosedXML** for XLSX.
- [ ] Implement validation rules (required fields, status enum, reason code whitelist).
- [ ] Implement **upsert**: `Part` by `PartNumber`, `PR` by `PR_SourceId` (or hash fallback).
- [ ] Implement logging + move processed files to `*.done` or `*.err` with reason.
- [ ] Write **rejects CSV** for invalid rows.
- [ ] Configurable via `appsettings.json` (`Ingest:WatchDir`, batch size, delay).

**Acceptance Criteria:**
- Dropping a valid CSV creates/updates rows and leaves an audit trail of the run; rejects file generated for bad rows.

---

## 6) Optional Exporter (UI Automation → CSV) — Only If Approved

- [ ] Create `APT.ScrapeExporter` worker (Playwright or Selenium).
- [ ] Use **persistent profile** (no hardcoded creds); first run headful for SSO/MFA.
- [ ] Navigate to iPLM/ERP report page; trigger **Export CSV**; save to `Ingest:WatchDir`.
- [ ] Implement simple **retry** with backoff; archive downloaded files.
- [ ] Add **selector config** file for easy maintenance.
- [ ] Document policy exception/approval email; provide **manual export fallback**.

**Acceptance Criteria:**
- When allowed, exporter produces a valid CSV at the watch directory without storing passwords; when blocked, manual export path still works.

**Compliance Notes:**
- Do not scrape raw tables if official export exists.
- No credentials in code; no bypass of MFA.

---

## 7) Frontend (React + TypeScript + Vite) — **Detailed Plan**

### 7.1 Project Setup
- [ ] Ensure Node 18+ and npm installed.
- [ ] In `frontend/apt-web/`, ensure these exist: `package.json`, `index.html`, `vite.config.ts`, `tsconfig.json`, `src/*`.
- [ ] Create `.env.local` with `VITE_API_BASE=http://localhost:5200/api` (or your API URL).
- [ ] Install deps and run dev server:
  ```bash
  npm install
  npm run dev
  ```

**Acceptance:** App loads at `http://localhost:5173`.

### 7.2 Directory Structure
```
src/
  api/
    client.ts       # axios instance
    types.ts        # DTOs/Types
    hooks.ts        # react-query hooks (usePRs, usePR, useUpdatePRStatus)
  components/
    Layout.tsx      # header + outlet
    StatusChip.tsx  # status colored badge
  pages/
    PRList.tsx      # list + filters + link to detail
    PRDetail.tsx    # detail + status update + history
  styles.css
  main.tsx          # router + providers
```

### 7.3 Routing & State
- [ ] Use `react-router-dom` with a top-level `Layout` and nested routes.
- [ ] Use **TanStack Query** for server state (caching, retries) — no global store unless needed.

**Acceptance:** Navigating list→detail retains cache; back is instant.

### 7.4 API Client & Hooks
- [ ] `axios` base URL from `VITE_API_BASE`.
- [ ] Hooks:
  - `usePRs({ reasonCode, status, skip, take })` — list with filters & paging
  - `usePR(id)` — detail including history
  - `useUpdatePRStatus(id)` — PATCH status, invalidate caches

**Acceptance:** Network calls hit `/api/prs` & `/api/prs/:id` as expected.

### 7.5 UI/UX Behavior
- [ ] **List page**
  - Filters: Status (dropdown), ReasonCode default `ALT_PART`.
  - Table columns: ID (link), Part, Status (chip), Owner, Opened.
  - Loading state card; error message on failure.
  - (Optional) Paging controls (Next/Prev) when API supports it.

- [ ] **Detail page**
  - Show: Part, Reason, Status (chip), Owner, Opened, SLA Due.
  - **Update Status**: dropdown of allowed statuses; comments input; submit button.
  - Show success/error feedback.
  - **History**: list items `ChangedAt → NewStatus by UPN (Comments)`.

- [ ] **Visuals**
  - Minimal CSS (provided). Keep it clean and legible.
  - Use monospace for IDs; chips for status.

**Acceptance:** End users can filter, view details, update status, and see history immediately.

### 7.6 Validation & Errors
- [ ] Disable submit while mutation in flight.
- [ ] Show server error message from API body `{ error: "..." }`.
- [ ] Basic client checks: block empty `newStatus`.

**Acceptance:** Invalid transition displays API error; UI remains responsive.

### 7.7 KPIs (No Power BI)
- [ ] Add a light KPI strip on PR List using current data:
  - Backlog count (length of list or API aggregate if available)
  - Aging buckets (client-side calc from `OpenedDate` until proper aggregates exist)
  - SLA breach % (if `SlaDue` present)

**Acceptance:** KPIs render and update with filters; no heavy libs required.

### 7.8 Accessibility & UX Polish
- [ ] Ensure focus states on links/buttons; keyboard navigation works.
- [ ] Labels for inputs; alt text where applicable.
- [ ] Color contrast adequate for status chips.

**Acceptance:** Basic a11y checks pass (no obvious blockers).

### 7.9 Configuration & Environments
- [ ] `.env.local` for dev; document `VITE_API_BASE`.
- [ ] For Test/Prod builds, use environment‑specific env files or CI variables.

**Acceptance:** Switching API URL requires no code changes.

### 7.10 Testing (Frontend)
- [ ] Unit tests (optional, if time): render list & detail with mocked hooks.
- [ ] E2E smoke (optional): Cypress/Playwright to open list, open detail, perform status update (mock API).

**Acceptance:** At least a manual test script exists in `/docs/wi-frontend.md` and is followed in UAT.

### 7.11 Performance & Reliability
- [ ] Use React Query cache to avoid refetch on back navigation.
- [ ] Avoid unnecessary re-renders; keep components simple.
- [ ] Add `suspense`/loading states where possible.

**Acceptance:** List and detail render in < 2s on typical dev hardware with 5–10k rows (paged).

### 7.12 Frontend Definition of Done
- [ ] Navigable list and detail views.
- [ ] Status update works (validations + visual feedback).
- [ ] History visible and correct.
- [ ] Basic KPIs visible.
- [ ] Env switching works.
- [ ] Minimal a11y and performance checks pass.

---

## 8) Authentication & Authorization (RBAC)

- [ ] Add Microsoft Entra ID auth to API (JWT) using `Microsoft.Identity.Web`.
- [ ] Map user UPN to `ChangedByUpn` for status changes.
- [ ] Role model: `APT.Viewer`, `APT.Editor`, `APT.Admin` (groups in Entra ID).
- [ ] Enforce authorization policies per endpoint.
- [ ] Frontend: handle 401/403; show/hide actions based on role.

**Acceptance Criteria:**
- Only authorized users can update PRs; history shows real UPN; viewers can’t mutate.

---

## 9) Observability & Ops

- [ ] Structured logging (Serilog or built‑in) with correlation id.
- [ ] Log sinks: console + file (dev); AppInsights (cloud) later.
- [ ] Health endpoints: `/health` for API and workers.
- [ ] Basic metrics (counts per run, rejects count) in logs.
- [ ] Error handling policy with retry where safe (ingest/exporter).

**Acceptance Criteria:**
- Operators can see success/failure of ingestion/export runs, with counts; API health reports OK.

---

## 10) Testing & Quality

- [ ] **Unit tests**: status transition rules; ingestion validators.
- [ ] **Integration tests**: in‑memory DB for API flow; sample CSV ingest end‑to‑end.
- [ ] **Load sanity**: list endpoint with paging on 5–10k rows <2s.
- [ ] **Security tests**: unauthorized update is blocked; audit history always written on changes.

**Acceptance Criteria:**
- CI runs tests successfully; coverage on core rules; no critical warnings.

---

## 11) CI/CD & Branching

- [ ] Create Git branching policy (feature → PR → main). Require build + tests on PR.
- [ ] Add CI (GitHub Actions/Azure DevOps): restore, build, test, publish artifacts.
- [ ] CD (optional): infra as code later; for now, manual deploy steps documented in `/ops/deploy.md`.
- [ ] Version EF migrations; tag releases.

**Acceptance Criteria:**
- Every PR triggers CI; main is always deployable; migrations are reviewed and versioned.

---

## 12) UAT (Monitoring & Validation)

- [ ] Prepare UAT dataset (real exports or sanitized).
- [ ] UAT scripts: create/update PR, invalid transition, export read‑only, view aging.
- [ ] Reconciliation: counts by status/reason/plant vs Excel tracker; spot‑check records.
- [ ] Capture evidence (screens, logs) and sign‑off notes.

**Acceptance Criteria:**
- Business confirms data parity and usability; defects triaged and fixed; sign‑off recorded in `/docs/uat-signoff.md`.

---

## 13) Release, Cutover & Training

- [ ] Announce cutover window; freeze Excel edits during import.
- [ ] Perform final import via ingestion worker.
- [ ] Validate key dashboards (counts, aging) post‑import.
- [ ] Deliver training: quick guide + WI with screenshots.
- [ ] Hypercare: define time window and escalation path.

**Deliverables:** `/docs/cutover-plan.md`, `/docs/WI.md`, `/docs/training.pdf`, release notes.

---

## 14) Post‑Go‑Live Operations

- [ ] Monitor ingestion success; track rejects and fix root causes with data owners.
- [ ] Backup & DR validation (restore test in lower env).
- [ ] Patch cadence and dependency updates.
- [ ] Access reviews (RBAC) per policy.

**Acceptance Criteria:**
- Stable ingestion with declining rejects; periodic backup/restore validated; no unauthorized access.

---

## 15) Documentation Pack (Repo)

- [ ] `README.md` — overview, quick start, architecture.
- [ ] `WHAT_TO_DO.md` — this file.
- [ ] `/docs/WI.md` — Work Instruction (user steps).
- [ ] `/docs/wi-frontend.md` — Frontend user + tester guide (screens/flows).
- [ ] `/docs/security.md` — RBAC, audit, data flow, compliance notes.
- [ ] `/docs/api.md` — endpoint contracts & examples.
- [ ] `/ops/sql/bootstrap.sql` — DB bootstrap.
- [ ] `/ops/deploy.md` — manual deploy steps.

**Acceptance Criteria:**
- New team member can set up and understand the system in under an hour using docs.

---

## 16) Backlog (Post‑MVP Enhancements)

- Aggregates API for richer charts (aging buckets, SLA breach trend, supplier/plant filters).
- Teams notifications (webhook) for SLA breaches and hot items.
- Reason Code master table and admin UI.
- Supplier/Site dimensions; scoped RBAC by plant.
- Soft‑delete & restore for records with evidence.
- Direct ERP/iPLM API connectors when access is approved (replace file ingest).
- Containerization & k8s manifests; Infra‑as‑Code (Bicep/Terraform) for Azure.

---

## 17) Quick Checklists

### Engineering Setup
- [ ] .NET 8 SDK, Node 18+, SQL Server/Docker.
- [ ] EF tools installed (`dotnet tool install dotnet-ef`).
- [ ] Connection strings set in API/Worker `appsettings.json`.

### Data Ingest Smoke Test
- [ ] Place sample CSV in `Ingest:WatchDir`.
- [ ] Worker logs show N processed, M upserts, K rejects.
- [ ] API shows new PRs; UI lists them.

### Status Update Smoke Test
- [ ] Open PR in UI; change status with comment.
- [ ] API returns 204; history shows new entry with `ChangedByUpn`.

---

## 18) Communication Templates

**Scraping Approval (if needed)**
```
Subject: Request approval – Automated Export (Read‑only) for APT PRs

We propose a headless Playwright job to navigate to the existing iPLM report page and click the official “Export CSV” button on a scheduled basis. No passwords in code; SSO/MFA done once with a persistent profile. Data lands in a secured drop folder and is ingested by our system. If not approved, we will proceed with manual CSV export by APT/Analytics.
```

**Cutover Notice**
```
Subject: APT Alternate Parts System – Cutover

We will freeze Excel edits during the cutover window. Final export will be ingested and validated. Post‑cutover, all updates happen via the web app. Excel will be available as read‑only export.
```

---

## 19) Acceptance Summary (End of Project)

- [ ] System live with controlled writes through API only.
- [ ] ≥50% manual effort eliminated (measured via baseline vs app logs).
- [ ] 100% of status changes captured in `PRStatusHistory` with user identity.
- [ ] UAT sign‑off stored; training delivered; support model in place.
- [ ] Documentation complete and stored in repo.

---

**You’re done when:** business is using the app for daily operations, Excel is read‑only, ingestion is stable, and you have UAT sign‑off + release notes.

