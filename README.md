
# APT Alternate Parts Monitoring – Full-Code Solution

## 📌 Overview
The Alternates Part Team (APT) currently tracks **Shortages**, **Problem Reports (PRs)**, and **Deviations** for Alternate Part reason codes in Excel. This approach is manual, error-prone, and lacks auditability. Our goal is to replace Excel with a **controlled, auditable, full-code system** that ensures data integrity, compliance, and efficiency.

---

## ✅ Updated Scope
- **Full-code solution** (no Power BI, no Power Automate)
- Centralized **SQL database** for all parts, PRs, shortages, and deviations
- **ASP.NET Core REST API** for controlled updates and validations
- **React + TypeScript UI** for search, filters, status updates, and dashboards
- **Background ingestion worker** for CSV/XLSX exports from ERP/PLM or manual drop
- **Optional Playwright exporter** for automated CSV download (if approved)
- **Audit trail** for all status changes (append-only history)
- **RBAC-ready** for Microsoft Entra ID integration
- **LAM-compliant**: lifecycle governance, UAT evidence, release controls

---

## 🏗 Architecture
```
ERP/PLM (or manual export) → Ingestion Worker → SQL Database → ASP.NET Core API → React UI
(Optional) Playwright Exporter → feeds Ingestion Worker
```

---

## 🧰 Tech Stack
- **Backend**: .NET 8, ASP.NET Core, EF Core 9, SQL Server
- **Frontend**: React + TypeScript + Vite + TanStack Query
- **Workers**: .NET Background Service for ingestion; optional Playwright exporter
- **Auth**: Microsoft Entra ID (future phase)
- **Hosting**: Azure-ready (App Service + Azure SQL)

---

## 🔑 Features
- Normalized schema for Parts, PRs, Shortages, Deviations, and Status History
- REST API with:
  - PR list & detail endpoints
  - Status update with validation and audit logging
- React UI with:
  - PR list, filters, detail view, status update
  - Basic KPIs (backlog count, aging buckets, SLA breach %)
- Ingestion worker:
  - Validates and deduplicates CSV/XLSX
  - Upserts into SQL; logs rejects
- Optional Playwright exporter:
  - Automates UI export → CSV → ingestion pipeline
- Compliance:
  - RBAC-ready, audit trails, immutable history

---

## ⚡ Setup Instructions

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server (local or Docker)

### Steps
1. **Clone repo & restore dependencies**
```bash
dotnet restore
npm install (in frontend)
```

2. **Start SQL (Docker)**
```bash
cd ops
docker compose up -d sql
```

3. **Apply migrations**
```bash
cd backend/src/APT.Api
dotnet ef migrations add InitialCreate --project ../APT.Infrastructure --startup-project .
dotnet ef database update --project ../APT.Infrastructure --startup-project .
```

4. **Run API**
```bash
cd backend/src/APT.Api
dotnet run
```

5. **Run ingestion worker**
```bash
cd backend/workers/APT.IngestWorker
dotnet run
```

6. **Run frontend**
```bash
cd frontend/apt-web
npm run dev
```

---

## 🔐 Compliance & Audit
- All writes via API only; no uncontrolled edits
- Status transitions validated; history logged with user identity
- RBAC integration planned for v2
- UAT evidence and release notes documented in `/docs`

---

## 🗺 Roadmap
- ✅ MVP: DB + API + UI + ingestion worker
- ➡ RBAC with Entra ID
- ➡ Optional Playwright exporter for automated CSV download
- ➡ Aggregates API for richer dashboards
- ➡ Teams notifications for SLA breaches
- ➡ Direct ERP/iPLM API integration (post-approval)

---

**Definition of Done:**
- System live with controlled writes, audit history, and validated transitions
- Excel replaced by web app (read-only export only)
- UAT sign-off and documentation complete
