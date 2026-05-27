using MongoDB.Driver;

using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Interfaces;

namespace RecruitMatch.Infrastructure.Persistence.Repositories;

public class MatchRepository(IMongoDatabase database) : Repository<Match>(database, "matches"), IMatchRepository;