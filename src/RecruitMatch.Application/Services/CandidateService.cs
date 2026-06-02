using RecruitMatch.Application.DTOs.Requests;
using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;
using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Interfaces;
using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Application.Services;

public class CandidateService : ICandidateService
{
	private readonly ICandidateRepository _repository;

	public CandidateService(ICandidateRepository repository)
	{
		_repository = repository;
	}

	public async Task<CandidateResponse> CreateAsync(CreateCandidateRequest request, CancellationToken ct = default)
	{
		var email = new Email(request.Email);
		var candidate = new Candidate(request.Name, email, request.Resume, request.Skills);

		await _repository.AddAsync(candidate, ct);
		return ToResponse(candidate);
	}

	public async Task<CandidateResponse?> GetByIdAsync(string id, CancellationToken ct = default)
	{
		var candidate = await _repository.GetByIdAsync(id, ct)
			?? throw new KeyNotFoundException($"Candidato {id} não encontrado.");
		return ToResponse(candidate);
	}

	public async Task<IReadOnlyList<CandidateResponse>> GetAllAsync(CancellationToken ct = default)
	{
		var candidates = await _repository.GetAllAsync(ct);
		return candidates.Select(ToResponse).ToList();
	}

	public async Task<CandidateResponse> UpdateAsync(string id, UpdateCandidateRequest request,
		CancellationToken ct = default)
	{
		var candidate = await _repository.GetByIdAsync(id, ct)
										?? throw new KeyNotFoundException($"Candidato {id} não encontrado.");

		var email = new Email(request.Email);
		candidate.Update(request.Name, email, request.Resume, request.Skills);

		await _repository.UpdateAsync(candidate, ct);
		return ToResponse(candidate);
	}

	public async Task DeleteAsync(string id, CancellationToken ct = default)
	{
		var candidate = await _repository.GetByIdAsync(id, ct)
										?? throw new KeyNotFoundException($"Candidato {id} não encontrado.");

		await _repository.DeleteAsync(id, ct);
	}

	private static CandidateResponse ToResponse(Candidate candidate) => new(
		candidate.Id,
		candidate.Name,
		candidate.Email.Address,
		candidate.Resume,
		candidate.Skills,
		candidate.CreatedAt,
		candidate.UpdatedAt
	);
}