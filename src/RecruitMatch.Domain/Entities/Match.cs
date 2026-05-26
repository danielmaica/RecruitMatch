using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Domain.Entities;

public class Match : AggregateRoot
{
	public string JobId { get; }
	public string CandidateId { get; }
	public string CandidateName { get; }
	public MatchScore Score { get; }
	public string ResumeSummary { get; }
	public string Justification { get; }
	public IReadOnlyList<string> Strengths { get; }
	public IReadOnlyList<string> Gaps { get; }
	public DateTime AnalyzedAt { get; }

	public Match(string jobId, string candidateId, string candidateName, MatchScore score, string resumeSummary, string justification, IReadOnlyList<string> strengths, IReadOnlyList<string> gaps)
	{
		JobId = string.IsNullOrWhiteSpace(jobId) ? throw new ArgumentException("JobId não pode ser vazio.", nameof(jobId)) : jobId;
		CandidateId = string.IsNullOrWhiteSpace(candidateId) ? throw new ArgumentException("CandidateId não pode ser vazio.", nameof(candidateId)) : candidateId;
		CandidateName = string.IsNullOrWhiteSpace(candidateName) ? throw new ArgumentException("Nome do candidato não pode ser vazio.", nameof(candidateName)) : candidateName;
		Score = score;
		ResumeSummary = string.IsNullOrWhiteSpace(resumeSummary) ? throw new ArgumentException("Resumo do currículo não pode ser vazio.", nameof(resumeSummary)) : resumeSummary;
		Justification = string.IsNullOrWhiteSpace(justification) ? throw new ArgumentException("Justificativa não pode ser vazio.", nameof(justification)) : justification;
		Strengths = strengths ?? [];
		Gaps = gaps ?? [];
		AnalyzedAt = DateTime.UtcNow;
	}
}
