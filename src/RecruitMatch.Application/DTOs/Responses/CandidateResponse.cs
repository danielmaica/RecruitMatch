namespace RecruitMatch.Application.DTOs.Responses;

public record CandidateResponse(
    string Id,
    string Name,
    string Email,
    string Resume,
    IReadOnlyList<string> Skills,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
