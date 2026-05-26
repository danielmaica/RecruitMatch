# RecruitMatch

API de análise de fit entre candidatos e vagas usando IA (Groq).

## Stack
- .NET 9
- MongoDB (MongoDB.Driver, sem atributos no Domain — usar BsonClassMap na Infrastructure)
- Groq API via HTTP (modelo a definir, ex: llama-3.3-70b-versatile)
- Swagger

## Arquitetura
Clean Architecture + DDD. Regra de dependência:
```
API → Application → Domain
Infrastructure → Application
Infrastructure → Domain
API → Infrastructure (só para registrar DI)
```

## Estrutura de pastas
```
src/
├── RecruitMatch.Domain/
│   ├── Entities/         Entity.cs, AggregateRoot.cs, Job.cs, Candidate.cs, Match.cs
│   ├── ValueObjects/     Email.cs, JobRequirements.cs, MatchScore.cs
│   ├── Enums/            Seniority.cs
│   └── Interfaces/       IRepository.cs, IJobRepository.cs, ICandidateRepository.cs, IMatchRepository.cs
├── RecruitMatch.Application/
│   ├── DTOs/
│   │   ├── Requests/     CreateJobRequest, UpdateJobRequest, RegisterCandidateRequest, UpdateCandidateRequest
│   │   └── Responses/    JobResponse, CandidateResponse, MatchResponse, MatchAIResult
│   ├── Interfaces/       IJobService, ICandidateService, IMatchService, IAIMatchService
│   └── Services/         JobService.cs, CandidateService.cs, MatchService.cs
├── RecruitMatch.Infrastructure/
│   ├── AI/               GroqMatchService.cs, GroqSettings.cs, PromptTemplates.cs
│   └── Persistence/
│       └── Repositories/ MongoJobRepository, MongoCandidateRepository, MongoMatchRepository (pendente)
└── RecruitMatch.API/
    └── Controllers/      JobsController, CandidatesController, MatchesController (pendente)
```

## Decisões tomadas
- `Entity` e `AggregateRoot` são `abstract class`; Value Objects são `record`
- Setters são `protected set` nas entidades, só `get` nos records
- `Id` usa `Guid.NewGuid().ToString()` — não ObjectId do Mongo (mantém Domain agnóstico de banco)
- `UpdatedAt` é `DateTime?` — null significa "nunca atualizado"
- `IsDeleted` na Entity base para soft delete
- `OnUpdate()` é `protected` — só chamado internamente pelas entidades
- Mapeamento Domain → DTO feito via `ToResponse()` privado nos Services (sem AutoMapper)
- `IReadOnlyList` nas coleções de resposta — nunca retornar null, retornar lista vazia
- MongoDB sem atributos no Domain — BsonClassMap será registrado na Infrastructure
- Seniority trafega como string nos DTOs, convertido via Enum.TryParse nos Services
- MatchService chama a IA sequencialmente (um candidato por vez) — intencional no MVP
- Sem CQRS — padrão de Services direto

## Endpoints planejados
- `POST   /api/jobs`                  → criar vaga
- `GET    /api/jobs`                  → listar vagas
- `GET    /api/jobs/{id}`             → buscar vaga
- `PUT    /api/jobs/{id}`             → atualizar vaga
- `DELETE /api/jobs/{id}`             → deletar vaga
- `POST   /api/candidates`            → cadastrar candidato
- `GET    /api/candidates`            → listar candidatos
- `GET    /api/candidates/{id}`       → buscar candidato
- `PUT    /api/candidates/{id}`       → atualizar candidato
- `DELETE /api/candidates/{id}`       → deletar candidato
- `POST   /api/jobs/{id}/analyze`     → analisar todos candidatos para a vaga
- `GET    /api/jobs/{id}/matches`     → buscar resultados de análise da vaga

## Status atual
- [x] Domain completo (entidades, value objects, enums, interfaces)
- [x] Application completo (DTOs, interfaces, services)
- [x] Infrastructure — GroqMatchService (AI/GroqMatchService.cs, GroqSettings.cs, PromptTemplates.cs)
- [ ] Infrastructure — repositórios MongoDB
- [ ] API — controllers, Program.cs, DI, Swagger
- [ ] Deploy

## Próximo passo
Criar os repositórios MongoDB em `src/RecruitMatch.Infrastructure/Persistence/Repositories/`.
- Classe base `MongoRepository<T>` implementando `IRepository<T>` (injetar `IMongoDatabase`)
- `MongoJobRepository`, `MongoCandidateRepository`, `MongoMatchRepository`
- `BsonClassMap` para mapear entidades do Domain (sem atributos no Domain)
- Registrar BsonClassMap numa classe estática separada (ex: `BsonMappings.cs`)
