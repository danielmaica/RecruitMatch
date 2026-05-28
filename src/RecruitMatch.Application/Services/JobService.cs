using RecruitMatch.Application.DTOs.Requests;
using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;
using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.Enums;
using RecruitMatch.Domain.Interfaces;
using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Application.Services;

public class JobService : IJobService
{
	private readonly IJobRepository _repository;

	public JobService(IJobRepository repository)
	{
		_repository = repository;
	}

	public async Task<JobResponse> CreateAsync(CreateJobRequest request, CancellationToken ct = default)
	{
		if (!Enum.TryParse<Seniority>(request.Seniority, ignoreCase: true, out var seniority))
			throw new ArgumentException($"Senioridade inválida: {request.Seniority}.", nameof(request.Seniority));

		var requirements = new JobRequirements(request.RequiredSkills, request.PreferredSkills);
		var job = new Job(request.Title, request.Description, seniority, requirements);

		await _repository.AddAsync(job, ct);
		return ToResponse(job);
	}

	public async Task<JobResponse?> GetByIdAsync(string id, CancellationToken ct = default)
	{
		var job = await _repository.GetByIdAsync(id, ct)
			?? throw new KeyNotFoundException($"Vaga {id} não encontrada.");
		return ToResponse(job);
	}

	public async Task<IReadOnlyList<JobResponse>> GetAllAsync(CancellationToken ct = default)
	{
		var jobs = await _repository.GetAllAsync(ct);
		return jobs.Select(ToResponse).ToList();
	}

	public async Task<JobResponse> UpdateAsync(string id, UpdateJobRequest request, CancellationToken ct = default)
	{
		var job = await _repository.GetByIdAsync(id, ct)
							?? throw new KeyNotFoundException($"Vaga {id} não encontrada.");

		if (!Enum.TryParse<Seniority>(request.Seniority, ignoreCase: true, out var seniority))
			throw new ArgumentException($"Senioridade inválida: {request.Seniority}.", nameof(request.Seniority));

		var requirements = new JobRequirements(request.RequiredSkills, request.PreferredSkills);
		job.Update(request.Title, request.Description, seniority, requirements);

		await _repository.UpdateAsync(job, ct);
		return ToResponse(job);
	}

	public async Task DeleteAsync(string id, CancellationToken ct = default)
	{
		var job = await _repository.GetByIdAsync(id, ct)
							?? throw new KeyNotFoundException($"Vaga {id} não encontrada.");

		await _repository.DeleteAsync(id, ct);
	}

	private static JobResponse ToResponse(Job job) => new(
		job.Id,
		job.Title,
		job.Description,
		job.Seniority.ToString(),
		job.Requirements.Required,
		job.Requirements.Preferred,
		job.CreatedAt,
		job.UpdatedAt
	);
}