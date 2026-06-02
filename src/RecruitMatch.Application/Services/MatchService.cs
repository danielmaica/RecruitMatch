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

	public async Task<MatchAnalysisResponse> AnalyzeAsync(string jobId, CancellationToken ct = default)
	{
		var job = await _jobRepository.GetByIdAsync(jobId, ct)
							?? throw new KeyNotFoundException($"Vaga {jobId} não encontrada");

		var candidates = await _candidateRepository.GetAllAsync(ct);

		if (candidates.Count == 0)
			throw new InvalidOperationException("Nenhum candidato cadastrado para analisar.");

		var analyzed = new List<MatchResponse>();
		var failures = new List<MatchFailure>();

		foreach (var candidate in candidates)
		{
			try
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
				analyzed.Add(ToResponse(match));
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				failures.Add(new MatchFailure(candidate.Id, candidate.Name, ex.Message));
			}
		}

		return new MatchAnalysisResponse(
			[.. analyzed.OrderByDescending(r => r.Score)],
			failures
		);
	}

	public async Task<IReadOnlyList<MatchResponse>> GetByJobIdAsync(string jobId, CancellationToken ct = default)
	{
		var matches = await _matchRepository.GetAllAsync(m => m.JobId == jobId, ct);
		return [.. matches.Select(ToResponse).OrderByDescending(r => r.Score)];
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