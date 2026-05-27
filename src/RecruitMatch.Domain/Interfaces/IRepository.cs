using System.Linq.Expressions;

using RecruitMatch.Domain.Entities;

namespace RecruitMatch.Domain.Interfaces;

public interface IRepository<T> where T : AggregateRoot
{
	Task AddAsync(T entity, CancellationToken ct = default);
	Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
	Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
	Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
	Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
	Task UpdateAsync(T entity, CancellationToken ct = default);
	Task DeleteAsync(string id, CancellationToken ct = default);
	Task PhysicalDeleteAsync(string id, CancellationToken ct = default);
}
