namespace RecruitMatch.Application.DTOs.Responses;

public record JobResponse(
	string Id,
	string Title,
	string Description,
	string Seniority,
	IReadOnlyList<string> RequiredSkills,
	IReadOnlyList<string> PreferredSkills,
	DateTime CreatedAt,
	DateTime? UpdatedAt
);
