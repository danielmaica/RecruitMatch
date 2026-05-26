using RecruitMatch.Domain.Entities;

namespace RecruitMatch.Domain.Interfaces;

public interface IMatchRepository : IRepository<Match>
{
	Task<IReadOnlyList<Match>> GetByJobIdAsync(string jobId, CancellationToken ct = default);
}
