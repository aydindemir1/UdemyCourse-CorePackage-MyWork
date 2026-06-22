# UdemyCourse — Core Package Suite

A production-ready collection of **25 reusable .NET 9.0 NuGet packages** providing the foundational building blocks for enterprise-grade microservice applications. This monorepo covers everything from CQRS and clean architecture abstractions to security, messaging, observability, and resilience.

---

## Packages Overview

### Foundation

| Package | Version | Description |
|---------|---------|-------------|
| [Core.Domain](./Core.Domain/README.md) | 3.0.2 | Base domain primitives — Entity, AggregateRoot, Value Object |
| [Core.Abstractions](./Core.Abstractions/README.md) | 3.1.9 | Interfaces for CQRS, repositories, events, and messaging |
| [Core.Cqrs](./Core.Cqrs/README.md) | 3.0.2 | MediatR-based CQRS dispatcher implementation |
| [Core.Application](./Core.Application/README.md) | 3.0.4 | MediatR pipeline behaviors (validation, authorization) |
| [Core.Extensions](./Core.Extensions/README.md) | 3.0.7 | Utility extension methods for auth, HTTP, and common types |
| [Core.WebApi](./Core.WebApi/README.md) | 3.0.3 | Web API helpers, response wrappers, and controller extensions |
| [Core.CrossCuttingConcerns.Exceptions](./Core.CrossCuttingConcerns.Exceptions/README.md) | 3.0.1 | Global exception handling with RFC 7807 Problem Details |

### Persistence

| Package | Version | Description |
|---------|---------|-------------|
| [Core.Persistence](./Core.Persistence/README.md) | 3.0.9 | Generic repositories for EF Core, MongoDB, and Redis |

### Security

| Package | Version | Description |
|---------|---------|-------------|
| [Core.Security.Domain](./Core.Security.Domain/README.md) | 3.0.1 | Security entities (User, Token, OTP) and enums |
| [Core.Security.Hashing](./Core.Security.Hashing/README.md) | 3.0.0 | BCrypt password hashing |
| [Core.Security.Encryption](./Core.Security.Encryption/README.md) | 3.0.0 | AES / RSA encryption utilities |
| [Core.Security.Jwt](./Core.Security.Jwt/README.md) | 3.0.0 | JWT access & refresh token generation and validation |
| [Core.Security.Extensions](./Core.Security.Extensions/README.md) | 3.0.0 | DI registration extensions for the security stack |
| [Core.Security.EmailAuthenticator](./Core.Security.EmailAuthenticator/README.md) | 3.0.0 | Email-based two-factor authentication (OTP codes) |
| [Core.Security.Redis](./Core.Security.Redis/README.md) | 3.0.2 | Redis-backed token revocation and session management |

### Messaging & Events

| Package | Version | Description |
|---------|---------|-------------|
| [Core.Messaging](./Core.Messaging/README.md) | 3.0.4 | Outbox pattern core — publisher, background processor, serialization |
| [Core.Messaging.Postgres](./Core.Messaging.Postgres/README.md) | 3.0.3 | PostgreSQL implementation of the outbox store |
| [Core.Messaging.Transport.RabbitMq](./Core.Messaging.Transport.RabbitMq/README.md) | 3.0.5 | RabbitMQ producer/consumer with Polly retry |
| [Core.Events](./Core.Events/README.md) | 3.0.3 | In-process domain event dispatching via MediatR |

### Infrastructure

| Package | Version | Description |
|---------|---------|-------------|
| [Core.Mailing](./Core.Mailing/README.md) | 3.0.2 | Email delivery via MailKit / SMTP |
| [Core.Scheduling.Hangfire](./Core.Scheduling.Hangfire/README.md) | 3.0.2 | Background job scheduling with Hangfire |
| [Core.Monitoring](./Core.Monitoring/README.md) | 3.0.0 | Health checks UI and Prometheus metrics exporter |
| [Core.Tracing](./Core.Tracing/README.md) | 3.0.1 | OpenTelemetry distributed tracing for HTTP, MediatR, and RabbitMQ |
| [Core.Resiliency](./Core.Resiliency/README.md) | 3.0.0 | Polly-based retry, circuit-breaker, and timeout policies |
| [Core.ElasticSearch](./Core.ElasticSearch/README.md) | 3.0.1 | Elasticsearch CRUD and search helpers via NEST |

---

## Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│                        Your Application                          │
├────────────────┬────────────────────────┬────────────────────────┤
│  Presentation  │      Application       │       Domain           │
│  Core.WebApi   │  Core.Application      │  Core.Domain           │
│                │  Core.Cqrs             │  Core.Abstractions     │
├────────────────┴────────────────────────┴────────────────────────┤
│                       Infrastructure                             │
│  Core.Persistence  │ Core.Messaging  │ Core.Security.*          │
│  Core.Mailing      │ Core.Events     │ Core.ElasticSearch        │
│  Core.Tracing      │ Core.Monitoring │ Core.Resiliency           │
│  Core.Scheduling.Hangfire                                        │
└──────────────────────────────────────────────────────────────────┘
```

---

## Technology Stack

| Category | Technology |
|----------|-----------|
| Runtime | .NET 9.0 |
| ORM | Entity Framework Core 9.0 |
| Databases | PostgreSQL, SQL Server, MongoDB, Redis |
| Search | Elasticsearch (NEST 7) |
| Messaging | RabbitMQ 7 |
| Scheduling | Hangfire 1.8 |
| Observability | OpenTelemetry 1.15, Prometheus, Health Checks UI |
| Security | JWT, BCrypt, AES/RSA |
| Validation | FluentValidation 12 |
| Resilience | Polly 8.6 |
| Email | MailKit 4.15 |
| Mediator | MediatR 12.5 |

---

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 v17+ or JetBrains Rider

### Build All Packages

```bash
dotnet build UdemyCourse-CorePackage.sln --configuration Release
```

Built `.nupkg` files are placed in the `Packages/` directory at the repository root.

### Use a Local Package Source

```bash
dotnet nuget add source ./Packages --name CorePackages
```

Then reference packages in your project:

```xml
<PackageReference Include="Core.Persistence" Version="3.0.9" />
```

---

## Package Dependency Graph

```
Core.Domain
  └── Core.Security.Domain
        └── Core.Security.Hashing
        └── Core.Security.Encryption
              └── Core.Security.Jwt
        └── Core.Security.Redis

Core.Domain
  └── Core.Abstractions
        └── Core.Cqrs
        └── Core.Persistence
        └── Core.Messaging
              └── Core.Messaging.Postgres
              └── Core.Messaging.Transport.RabbitMq
        └── Core.Events
        └── Core.Extensions
              └── Core.WebApi
              └── Core.Application
```

---

## Contributing

1. Fork the repository and create a feature branch.
2. Follow existing conventions — each package is a self-contained class library.
3. Bump the `<Version>` in the `.csproj` using semantic versioning.
4. Open a pull request with a clear description of the change.

---

## License

This project is created for educational purposes as part of the Udemy course curriculum.
