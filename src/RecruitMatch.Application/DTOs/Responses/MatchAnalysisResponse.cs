namespace RecruitMatch.Application.DTOs.Responses;

public record MatchAnalysisResponse(
		IReadOnlyList<MatchResponse> Analyzed,
		IReadOnlyList<MatchFailure> Failures
);
