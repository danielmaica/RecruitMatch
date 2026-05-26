namespace RecruitMatch.Application.DTOs.Requests;

public record UpdateCandidateRequest(
	string Name,
	string Email,
	string Resume,
	IEnumerable<string> Skills
);
