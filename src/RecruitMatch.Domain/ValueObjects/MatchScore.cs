namespace RecruitMatch.Domain.ValueObjects;

public record MatchScore
{
	public int Value { get; }

	public MatchScore(int value)
	{
		if (value < 0 || value > 100)
			throw new ArgumentException($"O score deve estar entre 0 e 100. Valor fornecido: {value}");

		Value = value;
	}
}
