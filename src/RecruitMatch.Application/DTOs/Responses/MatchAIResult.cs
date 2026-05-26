namespace RecruitMatch.Application.DTOs.Responses;

public record MatchAIResult(
    int Score,
    string ResumeSummary,
    string Justification,
    IReadOnlyList<string> Strengths,
    IReadOnlyList<string> Gaps
);
