using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;
using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Interfaces;
using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Application.Services;

public class MatchService : IMatchService
{
	private readonly IMatchRepository _matchRepository;
	private readonly IJobRepository _jobRepository;
	private readonly ICandidateRepository _candidateRepository;
	private readonly IAIMatchService _aiMatchService;

	public MatchService(
		IMatchRepository matchRepository,
		IJobRepository jobRepository,
		ICandidateRepository candidateRepository,
		IAIMatchService aiMatchService
	)
	{
		_matchRepository = matchRepository;
		_jobRepository = jobRepository;
		_candidateRepository = candidateRepository;
		_aiMatchService = aiMatchService;
	}

	public async Task<IReadOnlyList<MatchResponse>> AnalyzeAsync(string jobId, CancellationToken ct = default)
	{
		var job = await _jobRepository.GetByIdAsync(jobId, ct)
							?? throw new KeyNotFoundException($"Vaga {jobId} não encontrada");

		var candidates = await _candidateRepository.GetAllAsync(ct);

		if (candidates.Count == 0)
			throw new InvalidOperationException("Nenhum candidato cadastrado para analisar.");

		var results = new List<MatchResponse>();

		foreach (var candidate in candidates)
		{
			var aiResult = await _aiMatchService.AnalyzeAsync(job, candidate, ct);

			var match = new Match(
				jobId,
				candidate.Id,
				candidate.Name,
				new MatchScore(aiResult.Score),
				aiResult.ResumeSummary,
				aiResult.Justification,
				aiResult.Strengths,
				aiResult.Gaps
			);

			await _matchRepository.AddAsync(match, ct);
			results.Add(ToResponse(match));
		}

		return results;
	}

	public async Task<IReadOnlyList<MatchResponse>> GetByJobIdAsync(string jobId, CancellationToken ct = default)
	{
		var matches = await _matchRepository.GetAllAsync(m => m.JobId == jobId, ct);
		return [.. matches.Select(ToResponse)];
	}

	private static MatchResponse ToResponse(Match match) => new(
		match.Id,
		match.JobId,
		match.CandidateId,
		match.CandidateName,
		match.Score.Value,
		match.ResumeSummary,
		match.Justification,
		match.Strengths,
		match.Gaps,
		match.AnalyzedAt
	);
}