namespace RecruitMatch.Application.DTOs.Responses;

public record MatchResponse(
    string Id,
    string JobId,
    string CandidateId,
    string CandidateName,
    int Score,
    string ResumeSummary,
    string Justification,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> Gaps,
    DateTime AnalyzedAt
);
