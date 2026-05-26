using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Domain.Entities;

namespace RecruitMatch.Application.Interfaces;

public interface IAIMatchService
{
  Task<MatchAIResult> AnalyzeAsync(Job job, Candidate candidate, CancellationToken ct = default);
}