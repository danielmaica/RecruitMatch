using RecruitMatch.Domain.Entities;

namespace RecruitMatch.Domain.Interfaces;

public interface IRepository<T> where T : AggregateRoot
{
	Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
	Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
	Task AddAsync(T entity, CancellationToken ct = default);
	Task UpdateAsync(T entity, CancellationToken ct = default);
	Task DeleteAsync(string id, CancellationToken ct = default);
}
