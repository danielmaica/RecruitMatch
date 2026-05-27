using MongoDB.Driver;

using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Interfaces;

namespace RecruitMatch.Infrastructure.Persistence.Repositories;

public class JobRepository(IMongoDatabase database) : Repository<Job>(database, "jobs"), IJobRepository;
