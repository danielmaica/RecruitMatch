namespace RecruitMatch.Domain.ValueObjects;

public record Email
{
	public string Value { get; }

	public Email(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentException("Email não pode ser vazio.", nameof(value));

		if (!value.Contains('@') || !value.Contains('.'))
			throw new ArgumentException($"Email inválido: {value}", nameof(value));

		Value = value.Trim().ToLowerInvariant();
	}
}