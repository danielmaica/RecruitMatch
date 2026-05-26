namespace RecruitMatch.Domain.ValueObjects;

public record JobRequirements
{
	public IReadOnlyList<string> Required { get; }
	public IReadOnlyList<string> Preferred { get; }

	public JobRequirements(IEnumerable<string> required, IEnumerable<string> preferred)
	{
		Required = required.ToList() ?? [];
		Preferred = preferred.ToList() ?? [];
	}

}
