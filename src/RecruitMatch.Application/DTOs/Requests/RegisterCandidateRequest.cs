namespace RecruitMatch.Application.DTOs.Requests;

public record RegisterCandidateRequest(
	string Name,
	string Email,
	string Resume,
	IEnumerable<string> Skills
);
