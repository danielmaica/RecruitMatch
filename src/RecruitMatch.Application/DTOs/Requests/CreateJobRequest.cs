namespace RecruitMatch.Application.DTOs.Requests;

public record CreateJobRequest(
	string Title,
	string Description,
	string Seniority,
	IEnumerable<string> RequiredSkills,
	IEnumerable<string> PreferredSkills
);
