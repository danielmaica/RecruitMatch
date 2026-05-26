using RecruitMatch.Application.DTOs.Requests;
using RecruitMatch.Application.DTOs.Responses;

namespace RecruitMatch.Application.Interfaces;

public interface IJobService
{
    Task<JobResponse> CreateAsync(CreateJobRequest request, CancellationToken ct = default);
    Task<JobResponse?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<JobResponse>> GetAllAsync(CancellationToken ct = default);
    Task<JobResponse> UpdateAsync(string id, UpdateJobRequest request, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}