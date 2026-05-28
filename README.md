# RecruitMatch

API REST para análise de compatibilidade entre candidatos e vagas de emprego usando IA generativa (Groq / LLaMA 3.3).

Para cada vaga, a API avalia todos os candidatos cadastrados e retorna um ranking com score de 0 a 100, resumo do perfil, justificativa, pontos fortes e gaps.

---

## Stack

- **.NET 9** — ASP.NET Core Web API
- **MongoDB** — persistência (MongoDB.Driver)
- **Groq API** — inferência com o modelo `llama-3.3-70b-versatile`
- **Scalar** — documentação interativa dos endpoints

---

## Arquitetura

Clean Architecture + DDD. A regra de dependência é rígida: camadas externas conhecem internas, nunca o contrário.

```
API  →  Application  →  Domain
         ↑
   Infrastructure
```

| Camada | Responsabilidade |
|---|---|
| **Domain** | Entidades, Value Objects, interfaces de repositório — zero dependências externas |
| **Application** | Casos de uso (Services), DTOs, interfaces de serviço |
| **Infrastructure** | Implementações concretas: repositórios MongoDB, integração com Groq |
| **API** | Controllers, configuração de DI, middleware de exceções |

---

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MongoDB](https://www.mongodb.com/try/download/community) rodando em `localhost:27017`
- Chave de API do [Groq](https://console.groq.com) (gratuita)

---

## Como rodar localmente

**1. Clone o repositório**

```bash
git clone https://github.com/danielmaica/RecruitMatch.git
cd RecruitMatch
```

**2. Configure a chave do Groq**

Edite `src/RecruitMatch.API/appsettings.Development.json` e substitua o valor de `ApiKey`:

```json
{
  "GroqSettings": {
    "Uri": "https://api.groq.com/openai/v1/chat/completions",
    "ApiKey": "SUA_CHAVE_AQUI",
    "Model": "llama-3.3-70b-versatile"
  },
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "RecruitMatch"
  }
}
```

**3. Execute a API**

```bash
dotnet run --project src/RecruitMatch.API
```

A API sobe em `https://localhost:5001` (ou porta indicada no terminal).

**4. Acesse a documentação**

Abra `https://localhost:5001/scalar/v1` para explorar e testar todos os endpoints via Scalar.

---

## Endpoints

### Vagas

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/jobs` | Criar vaga |
| `GET` | `/api/v1/jobs` | Listar vagas |
| `GET` | `/api/v1/jobs/{id}` | Buscar vaga |
| `PUT` | `/api/v1/jobs/{id}` | Atualizar vaga |
| `DELETE` | `/api/v1/jobs/{id}` | Deletar vaga |

**Exemplo — criar vaga:**

```json
POST /api/v1/jobs
{
  "title": "Desenvolvedor Backend .NET",
  "description": "Desenvolvimento de APIs REST em .NET com foco em performance e escalabilidade.",
  "seniority": "Senior",
  "requiredSkills": [".NET", "C#", "APIs REST", "SQL"],
  "preferredSkills": ["Azure", "Docker", "MongoDB"]
}
```

Valores aceitos para `seniority`: `Intern`, `Junior`, `Mid`, `Senior`, `Lead`.

---

### Candidatos

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/candidates` | Cadastrar candidato |
| `GET` | `/api/v1/candidates` | Listar candidatos |
| `GET` | `/api/v1/candidates/{id}` | Buscar candidato |
| `PUT` | `/api/v1/candidates/{id}` | Atualizar candidato |
| `DELETE` | `/api/v1/candidates/{id}` | Deletar candidato |

**Exemplo — cadastrar candidato:**

```json
POST /api/v1/candidates
{
  "name": "Ana Lima",
  "email": "ana.lima@email.com",
  "resume": "5 anos de experiência com .NET e C#. Trabalhou em sistemas de alta disponibilidade usando Azure e SQL Server. Tem experiência com Docker e CI/CD.",
  "skills": [".NET", "C#", "Azure", "SQL Server", "Docker"]
}
```

---

### Análise de Match

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/matches/analyze/{jobId}` | Analisar todos os candidatos para uma vaga |
| `GET` | `/api/v1/matches/{jobId}` | Buscar resultados de análise de uma vaga |

**Exemplo — analisar vaga:**

```
POST /api/v1/matches/analyze/{id-da-vaga}
```

Retorna os candidatos ordenados por score (maior primeiro):

```json
[
  {
    "id": "...",
    "jobId": "...",
    "candidateId": "...",
    "candidateName": "Ana Lima",
    "score": 87,
    "resumeSummary": "Profissional sênior com sólida experiência em .NET e ecossistema Microsoft.",
    "justification": "A candidata atende aos requisitos obrigatórios e demonstra experiência relevante com as tecnologias preferidas.",
    "strengths": [".NET e C# avançado", "Experiência com Azure", "Docker e CI/CD"],
    "gaps": ["Sem experiência explícita com MongoDB"],
    "analyzedAt": "2025-05-27T14:32:00Z"
  }
]
```

---

## Estrutura de pastas

```
src/
├── RecruitMatch.Domain/
│   ├── Entities/        Entity, AggregateRoot, Job, Candidate, Match
│   ├── ValueObjects/    Email, JobRequirements, MatchScore
│   ├── Enums/           Seniority
│   └── Interfaces/      IRepository<T>, IJobRepository, ICandidateRepository, IMatchRepository
├── RecruitMatch.Application/
│   ├── DTOs/
│   │   ├── Requests/    CreateJobRequest, UpdateJobRequest, CreateCandidateRequest, UpdateCandidateRequest
│   │   └── Responses/   JobResponse, CandidateResponse, MatchResponse, MatchAIResult
│   ├── Interfaces/      IJobService, ICandidateService, IMatchService, IAIMatchService
│   └── Services/        JobService, CandidateService, MatchService
├── RecruitMatch.Infrastructure/
│   ├── AI/              AIMatchService, PromptTemplates, GroqSettings
│   └── Persistence/
│       ├── BsonMappings.cs
│       ├── MongoDbSettings.cs
│       └── Repositories/  Repository<T>, JobRepository, CandidateRepository, MatchRepository
└── RecruitMatch.API/
    ├── Controllers/Api/V1/  JobsController, CandidatesController, MatchesController
    └── Program.cs
```

---

## Decisões de design

- **Domain agnóstico de banco** — sem atributos do MongoDB no Domain; mapeamento feito via `BsonClassMap` na Infrastructure
- **IDs como `Guid.ToString()`** — mantém o Domain desacoplado do MongoDB ObjectId
- **Soft delete** — entidades têm `IsDeleted`; `PhysicalDeleteAsync` disponível quando necessário
- **Predicados tipados** — repositórios aceitam `Expression<Func<T, bool>>` para queries sem vazar detalhes de persistência
- **Análise sequencial** — a IA processa um candidato por vez (intencional no MVP; paralelismo seria melhoria futura)
- **Erros mapeados globalmente** — `KeyNotFoundException` → 404, `ArgumentException` → 400, demais → 500
