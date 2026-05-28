namespace RecruitMatch.Application.DTOs.Requests;

public record CreateCandidateRequest(
	string Name,
	string Email,
	string Resume,
	IEnumerable<string> Skills
);
