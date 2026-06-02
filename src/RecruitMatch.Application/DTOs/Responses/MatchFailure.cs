namespace RecruitMatch.Application.DTOs.Responses;

public record MatchFailure(
		string CandidateId,
		string CandidateName,
		string Reason
);
