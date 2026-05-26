namespace RecruitMatch.Domain.Entities;

public abstract class Entity
{
	public string Id { get; protected set; } = Guid.NewGuid().ToString();
	public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
	public DateTime? UpdatedAt { get; protected set; }
	public int Version { get; protected set; } = 1;
	public bool IsDeleted { get; protected set; } = false;

	public void OnUpdate()
	{
		Version++;
		UpdatedAt = DateTime.UtcNow;
	}
}
