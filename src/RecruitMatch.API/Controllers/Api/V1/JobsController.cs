using Microsoft.AspNetCore.Mvc;

using RecruitMatch.Application.DTOs.Requests;
using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;

namespace RecruitMatch.API.Controllers.Api.V1
{
	[ApiController]
	[Route("api/v1/jobs")]
	[Produces("application/json")]
	public class JobsController : ControllerBase
	{
		private readonly IJobService _jobService;
		public JobsController(IJobService jobService)
		{
			_jobService = jobService;
		}

		/// <summary>Cria uma nova vaga</summary>
		/// <param name="request">Os dados para criação da vaga</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>A vaga criada</returns>
		[HttpPost]
		[ProducesResponseType(typeof(JobResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateJobRequest request, CancellationToken ct = default)
		{
			var createdJob = await _jobService.CreateAsync(request, ct);
			return Created($"/api/v1/jobs/{createdJob.Id}", createdJob);
		}

		/// <summary>Obtém uma vaga pelo ID</summary>
		/// <param name="id">O ID da vaga a ser obtida</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>A vaga obtida</returns>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken ct = default)
		{
			return Ok(await _jobService.GetByIdAsync(id, ct));
		}

		/// <summary>Obtém todas as vagas</summary>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>Lista de vagas</returns>
		[HttpGet]
		[ProducesResponseType(typeof(IReadOnlyList<JobResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
		{
			return Ok(await _jobService.GetAllAsync(ct));
		}

		/// <summary>Atualiza uma vaga existente</summary>
		/// <param name="id">O ID da vaga a ser atualizada</param>
		/// <param name="request">Os dados para atualização da vaga</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>A vaga atualizada</returns>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(JobResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateJobRequest request, CancellationToken ct = default)
		{
			return Ok(await _jobService.UpdateAsync(id, request, ct));
		}

		/// <summary>Exclui uma vaga</summary>
		/// <param name="id">O ID da vaga a ser excluída</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>Sucesso</returns>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct = default)
		{
			await _jobService.DeleteAsync(id, ct);
			return NoContent();
		}
	}
}