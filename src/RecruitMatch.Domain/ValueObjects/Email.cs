namespace RecruitMatch.Domain.ValueObjects;

public record Email
{
	public string Address { get; }

	public Email(string address)
	{
		if (string.IsNullOrWhiteSpace(address))
			throw new ArgumentException("Email não pode ser vazio.", nameof(address));

		if (!address.Contains('@') || !address.Contains('.'))
			throw new ArgumentException($"Email inválido: {address}", nameof(address));

		Address = address.Trim().ToLowerInvariant();
	}
}