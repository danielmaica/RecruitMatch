using MongoDB.Driver;

using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Interfaces;

namespace RecruitMatch.Infrastructure.Persistence.Repositories;

public class CandidateRepository(IMongoDatabase database) : Repository<Candidate>(database, "candidates"), ICandidateRepository;
