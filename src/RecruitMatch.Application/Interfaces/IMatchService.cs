using RecruitMatch.Application.DTOs.Responses;

namespace RecruitMatch.Application.Interfaces;

public interface IMatchService
{
    Task<IReadOnlyList<MatchResponse>> AnalyzeAsync(string jobId, CancellationToken ct = default);
    Task<IReadOnlyList<MatchResponse>> GetByJobIdAsync(string jobId, CancellationToken ct = default);
}