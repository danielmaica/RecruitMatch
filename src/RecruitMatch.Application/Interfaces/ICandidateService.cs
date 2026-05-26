using RecruitMatch.Application.DTOs.Requests;
using RecruitMatch.Application.DTOs.Responses;

namespace RecruitMatch.Application.Interfaces;

public interface ICandidateService
{
    Task<CandidateResponse> CreateAsync(RegisterCandidateRequest request, CancellationToken ct = default);
    Task<CandidateResponse?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<CandidateResponse>> GetAllAsync(CancellationToken ct = default);
    Task<CandidateResponse> UpdateAsync(string id, UpdateCandidateRequest request, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}