# EyeCare

Multi-branch eye clinic management system, built as a REST API with **ASP.NET Core 9** and **Clean Architecture**.

EyeCare covers the full clinic workflow: patients, doctors and their schedules, appointment booking across branches, medical records with an audit trail, invoicing, payments and refunds. Access is controlled by roles, permissions and branch membership.

[![CI/CD](https://github.com/ahmed19104/EyeCare/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/ahmed19104/EyeCare/actions/workflows/ci-cd.yml)
![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-EF%20Core%209-CC2927)
![Redis](https://img.shields.io/badge/Redis-cache-DC382D)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED)

## Table of contents

- [Features](#features)
- [Architecture](#architecture)
- [Tech stack](#tech-stack)
- [Project structure](#project-structure)
- [API overview](#api-overview)
- [Roles and permissions](#roles-and-permissions)
- [Domain model](#domain-model)
- [Getting started](#getting-started)
- [Configuration](#configuration)
- [Testing](#testing)
- [CI/CD](#cicd)
- [Observability](#observability)

## Features

**Clinic operations**
- Multiple branches; doctors can work in several branches, each with its own weekly schedule.
- Patient management (create, read, list, update, delete).
- Available-slot lookup per doctor and branch.

**Appointments**
- Book, check in, start, complete, cancel and mark as no-show.
- Status flow: `Scheduled -> CheckedIn -> InProgress -> Completed`, plus `Cancelled` and `NoShow`.
- Booking validates the doctor, branch, patient and slot availability through specifications.

**Medical records**
- Create, read and update records per patient.
- Audit history for every record (`AuditLog`).

**Billing**
- Invoices with line items; draft, then issue.
- Payments by `Cash`, `Visa` or `BankTransfer`, with payment attempts and status tracking (`Pending`, `Succeeded`, `Failed`, `Cancelled`).
- Refunds with a configurable refund window (30 days by default) and refund attempts.
- Payment and refund **webhooks** with signature validation, plus **reconcile** endpoints.
- **Idempotency keys** on payments and refunds to prevent double charges.

**Security**
- ASP.NET Core Identity with **JWT** access tokens (60 min) and **refresh tokens** (7 days, hashed, revocable).
- Role-based access plus fine-grained **permission policies** and **branch-level authorization**.
- Rate limiting and global exception handling that returns ProblemDetails.

**Operations**
- Redis caching, health checks (`/health/live`, SQL Server and Redis readiness), Serilog structured logs, OpenTelemetry tracing to Jaeger.
- Docker Compose stack with an Nginx HTTPS reverse proxy.

## Architecture

The solution follows Clean Architecture. Dependencies point inward.

```
EyeCare (API)  ->  EyeCare.Application  ->  EyeCare.Domain
      |                    ^
      +-> EyeCare.Infrastructure
```

| Project | Responsibility |
| --- | --- |
| `EyeCare` (`EyeCareAPI`) | Controllers, middleware, rate limiting, health checks, Swagger, composition root |
| `EyeCare.Application` | Use cases as **CQRS** commands and queries (**MediatR**), **FluentValidation**, specifications, abstractions, pipeline behaviors (validation, tracing) |
| `EyeCare.Domain` | Entities, enums and business rules with no framework dependencies |
| `EyeCare.Infrastructure` | EF Core + SQL Server persistence, Identity, JWT, refresh tokens, Redis cache, authorization services, payment gateway client |
| `FakePaymentGateway` | Small minimal-API service that simulates a payment provider (delay and failure simulation) |
| `EyeCare.Domain.Tests`, `EyeCare.Application.Tests` | xUnit test projects |

Patterns used: CQRS, Specification, Repository and Unit of Work, pipeline behaviors, Options pattern, idempotent commands, webhook signature validation.

## Tech stack

- C# / .NET 9, ASP.NET Core Web API
- Entity Framework Core 9 (SQL Server), ASP.NET Core Identity
- MediatR 12, FluentValidation 12
- JWT Bearer authentication
- Redis (`Microsoft.Extensions.Caching.StackExchangeRedis`)
- Serilog, OpenTelemetry (ASP.NET Core, HTTP, SqlClient, OTLP), Jaeger
- Swagger / Swashbuckle
- xUnit
- Docker, Docker Compose, Nginx, GitHub Actions

## Project structure

```
EyeCare/
├── EyeCare/                    # API project (controllers, middleware, Program.cs)
├── EyeCare.Application/        # Commands, queries, validators, specifications, abstractions
│   ├── Appointments/  Billing/  Doctors/  Patients/  MedicalRecords/
│   ├── Identity/  Authorization/  Branches/  Common/Behaviors/
├── EyeCare.Domain/             # Entities, enums, domain exceptions
├── EyeCare.Infrastructure/     # Persistence, identity, caching, payments, authorization
├── EyeCare.Application.Tests/
├── EyeCare.Domain.Tests/
├── FakePaymentGateway/         # Simulated payment provider
├── nginx/nginx.conf            # HTTPS reverse proxy
├── Dockerfile
├── docker-compose.yml
├── .env.example
└── .github/workflows/ci-cd.yml
```

## API overview

Interactive docs are available in Swagger (`/swagger`) when running in Development.

| Area | Endpoints |
| --- | --- |
| Identity | `POST /api/identity/register`, `POST /api/identity/login`, `GET /api/identity/me`, `POST /api/identity/users/{userId}/roles`, `POST /api/identity/users/{userId}/branches/{branchId}` |
| Auth | `POST /api/auth/refresh`, `POST /api/auth/revoke-refresh-token` |
| Branches | `POST /api/branches` |
| Doctors | `POST /api/doctors`, `POST /api/doctors/{doctorId}/branches/{branchId}`, `POST /api/doctors/schedule`, `GET /api/doctors/{doctorId}/branches/{branchId}/available-slots` |
| Patients | `POST /api/patients`, `GET /api/patients`, `GET /api/patients/{id}`, `PUT /api/patients/{id}`, `DELETE /api/patients/{id}` |
| Appointments | `POST /api/appointments`, `POST /api/appointments/{id}/check-in`, `/start`, `/complete`, `/cancel`, `/no-show`, `GET /api/appointments/{id}`, `GET /api/appointments/doctor/{doctorId}`, `GET /api/appointments/patient/{patientId}` |
| Medical records | `POST /api/medical-records`, `GET /api/medical-records/{id}`, `PUT /api/medical-records/{id}`, `GET /api/medical-records/patient/{patientId}`, `GET /api/medical-records/{id}/audit-history` |
| Invoices | `POST /api/invoices`, `POST /api/invoices/{id}/items`, `POST /api/invoices/{id}/issue`, `GET /api/invoices/{id}` |
| Payments | `POST /api/payments`, `GET /api/payments/{id}`, `POST /api/payments/{id}/reconcile` |
| Refunds | `POST /api/refunds`, `POST /api/refunds/{id}/reconcile` |
| Webhooks | `POST /api/webhooks/payments`, `POST /api/webhooks/refunds` |
| Health | `/health/live`, readiness checks for SQL Server and Redis |

## Roles and permissions

Built-in roles: `Admin`, `BranchManager`, `Receptionist`, `Doctor`, `Accountant`.

Endpoints are protected by permission policies grouped by area (billing, appointments, medical records, doctors, identity and more), for example `Billing.Create`, `Billing.Read`, `Billing.Refund`, `MedicalRecords.Read`, `MedicalRecords.Update`. Roles and their permissions are seeded at startup (`IdentitySeeder`, `PermissionSeeder`). Users are also assigned to branches, and branch authorization limits what they can reach.

## Domain model

`Patient`, `Doctor`, `Branch`, `DoctorBranch`, `DoctorSchedule`, `Appointment`, `MedicalRecord`, `AuditLog`, `Invoice`, `InvoiceItem`, `Payment`, `PaymentAttempt`, `PaymentWebhookEvent`, `Refund`, `RefundAttempt`, `RefundWebhookEvent`, `RolePermission`, `UserBranch`, `RefreshToken`.

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (the Docker setup expects it on the host at port 1433)
- Docker Desktop (for Redis, Jaeger, Nginx and the containerized API)

### Run with Docker Compose

1. Copy the environment template and set your own values:
   ```bash
   cp .env.example .env
   ```
   Set `SQL_PASSWORD`, `JWT_KEY` and `PAYMENTS_WEBHOOK_SECRET`. Use your own strong secrets.
2. Make sure a local HTTPS certificate exists at `certs/localhost.pem` and `certs/localhost-key.pem` (for example generated with `mkcert`).
3. Start the stack:
   ```bash
   docker compose up --build
   ```

Services:

| Service | Address |
| --- | --- |
| API | `http://127.0.0.1:8080` |
| HTTPS proxy (Nginx) | `https://localhost` |
| Jaeger UI | `http://127.0.0.1:16686` |
| Redis | internal, `redis:6379` |

### Run locally without Docker

```bash
dotnet restore EyeCare.sln
dotnet run --project EyeCare
```

Set the SQL Server connection string (`ConnectionStrings:DefaultConnection`), Redis (`ConnectionStrings:Redis`) and `Jwt:Key` through user secrets or environment variables. The identity and permission seed data is applied on startup.

## Configuration

| Setting | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection |
| `ConnectionStrings:Redis` | Redis endpoint |
| `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpiryInMinutes` | JWT signing and lifetime |
| `RefreshToken:LifetimeDays` | Refresh token lifetime (7) |
| `Billing:Refund:WindowDays` | Refund window (30) |
| `OpenTelemetry:OtlpEndpoint` | Trace exporter endpoint |
| `DataProtection:KeysPath` | Data protection key storage |

> Never commit real secrets. Keep them in `.env`, user secrets or your CI secret store.

## Testing

```bash
dotnet test EyeCare.sln
```

The suite uses xUnit and covers the domain entities (appointments, branches, doctors, schedules, invoices, payments, refunds, webhook events, refresh tokens, permissions) and the appointment booking command handler.

## CI/CD

GitHub Actions (`.github/workflows/ci-cd.yml`):

1. On every push and pull request to `main`: restore, build and test the solution on .NET 9.
2. On push to `main`: build and push the Docker image to GitHub Container Registry as `ghcr.io/ahmed19104/eyecare` (tags `latest` and the commit SHA).

## Observability

- **Logs:** Serilog with request id and user id in every line.
- **Traces:** OpenTelemetry (ASP.NET Core, HTTP client, SQL client) exported over OTLP to Jaeger.
- **Health:** liveness at `/health/live`; readiness checks for SQL Server and Redis; the container image has a built-in `HEALTHCHECK`.

## Author

**Ahmed Yaser** - [@ahmed19104](https://github.com/ahmed19104)
