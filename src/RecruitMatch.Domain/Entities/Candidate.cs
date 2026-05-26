using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Domain.Entities;

public class Candidate : AggregateRoot
{
	public string Name { get; private set; }
	public Email Email { get; private set; }
	public string Resume { get; private set; }
	public IReadOnlyList<string> Skills { get; private set; }

	public Candidate(string name, Email email, string resume, IEnumerable<string> skills)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("O nome não pode ser vazio.", nameof(name));

		if (string.IsNullOrWhiteSpace(resume))
			throw new ArgumentException("O currículo não pode ser vazio.", nameof(resume));

		Name = name;
		Email = email ?? throw new ArgumentNullException(nameof(email));
		Resume = resume;
		Skills = skills.ToList() ?? [];
	}

	public void Update(string name, Email email, string resume, IEnumerable<string> skills)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("O nome não pode ser vazio.", nameof(name));

		if (string.IsNullOrWhiteSpace(resume))
			throw new ArgumentException("O currículo não pode ser vazio.", nameof(resume));

		Name = name;
		Email = email ?? throw new ArgumentNullException(nameof(email));
		Resume = resume;
		Skills = skills.ToList() ?? [];
		OnUpdate();
	}
}
