using RecruitMatch.Domain.Enums;
using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Domain.Entities;

public class Job : AggregateRoot
{
	public string Title { get; private set; }
	public string Description { get; private set; }
	public Seniority Seniority { get; private set; }
	public JobRequirements Requirements { get; private set; }

	public Job(string title, string description, Seniority seniority, JobRequirements requirements)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("O título não pode ser vazio.", nameof(title));

		if (string.IsNullOrWhiteSpace(description))
			throw new ArgumentException("A descrição não pode ser vazia.", nameof(description));

		Title = title;
		Description = description;
		Seniority = seniority;
		Requirements = requirements;
	}

	public void Update(string title, string description, Seniority seniority, JobRequirements requirements)
	{
		if (string.IsNullOrWhiteSpace(title))
			throw new ArgumentException("O título não pode ser vazio.", nameof(title));

		if (string.IsNullOrWhiteSpace(description))
			throw new ArgumentException("A descrição não pode ser vazia.", nameof(description));

		Title = title;
		Description = description;
		Seniority = seniority;
		Requirements = requirements;
		OnUpdate();
	}
}
