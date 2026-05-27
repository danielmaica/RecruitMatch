using System.Linq.Expressions;

using MongoDB.Driver;

using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Interfaces;

namespace RecruitMatch.Infrastructure.Persistence.Repositories;

public abstract class Repository<T> : IRepository<T> where T : AggregateRoot
{
	protected readonly IMongoCollection<T> _collection;

	public Repository(IMongoDatabase database, string collectionName)
	{
		_collection = database.GetCollection<T>(collectionName);
	}

	public Task AddAsync(T entity, CancellationToken ct = default) => _collection.InsertOneAsync(entity, cancellationToken: ct);

	public async Task<T?> GetByIdAsync(string id, CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.And(
			Builders<T>.Filter.Eq(e => e.Id, id),
			Builders<T>.Filter.Eq(e => e.IsDeleted, false)
		);

		return await _collection.Find(filter).FirstOrDefaultAsync(ct);
	}
	public async Task<T?> GetByIdAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.And(
			Builders<T>.Filter.Where(predicate),
			Builders<T>.Filter.Eq(e => e.IsDeleted, false)
		);

		return await _collection.Find(filter).FirstOrDefaultAsync(ct);
	}

	public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.Eq(e => e.IsDeleted, false);

		return await _collection.Find(filter).ToListAsync(ct);
	}

	public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.And(
			Builders<T>.Filter.Where(predicate),
			Builders<T>.Filter.Eq(e => e.IsDeleted, false)
		);

		return await _collection.Find(filter).ToListAsync(ct);
	}

	public async Task UpdateAsync(T entity, CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.And(
			Builders<T>.Filter.Eq(e => e.Id, entity.Id),
			Builders<T>.Filter.Eq(e => e.IsDeleted, false)
		);
		var updateResult = await _collection.ReplaceOneAsync(filter, entity, cancellationToken: ct);

		if (updateResult.MatchedCount == 0)
			throw new KeyNotFoundException($"ID {entity.Id} não encontrado.");
	}

	public async Task DeleteAsync(string id, CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.And(
			Builders<T>.Filter.Eq(e => e.Id, id),
			Builders<T>.Filter.Eq(e => e.IsDeleted, false)
		);
		var update = Builders<T>.Update.Set(e => e.IsDeleted, true);
		var updatedResult = await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);

		if (updatedResult.MatchedCount == 0)
			throw new KeyNotFoundException($"ID {id} não encontrado.");
	}

	public async Task PhysicalDeleteAsync(string id, CancellationToken ct = default)
	{
		var filter = Builders<T>.Filter.Eq(e => e.Id, id);
		var deleteResult = await _collection.DeleteOneAsync(filter, cancellationToken: ct);

		if (deleteResult.DeletedCount == 0)
			throw new KeyNotFoundException($"ID {id} não encontrado.");
	}

}
