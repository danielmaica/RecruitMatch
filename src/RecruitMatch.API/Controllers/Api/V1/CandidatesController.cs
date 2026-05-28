using Microsoft.AspNetCore.Mvc;

using RecruitMatch.Application.DTOs.Requests;
using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;

namespace RecruitMatch.API.Controllers.Api.V1
{
	[ApiController]
	[Route("api/v1/candidates")]
	[Produces("application/json")]
	public class CandidatesController : ControllerBase
	{
		private readonly ICandidateService _candidateService;
		public CandidatesController(ICandidateService candidateService)
		{
			_candidateService = candidateService;
		}

		/// <summary>Cria um novo candidato</summary>
		/// <param name="request">Os dados para criação do candidato</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>O candidato criado</returns>
		[HttpPost]
		[ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateAsync([FromBody] CreateCandidateRequest request, CancellationToken ct = default)
		{
			var createdCandidate = await _candidateService.CreateAsync(request, ct);
			return Created($"/api/v1/candidates/{createdCandidate.Id}", createdCandidate);
		}

		/// <summary>Obtém um candidato pelo ID</summary>
		/// <param name="id">O ID do candidato a ser obtido</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>O candidato obtido</returns>
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken ct = default)
		{
			return Ok(await _candidateService.GetByIdAsync(id, ct));
		}

		/// <summary>Obtém todos os candidatos</summary>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>Lista de candidatos</returns>
		[HttpGet]
		[ProducesResponseType(typeof(IReadOnlyList<CandidateResponse>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllAsync(CancellationToken ct = default)
		{
			return Ok(await _candidateService.GetAllAsync(ct));
		}

		/// <summary>Atualiza um candidato existente</summary>
		/// <param name="id">O ID do candidato a ser atualizado</param>
		/// <param name="request">Os dados para atualização do candidato</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>O candidato atualizado</returns>
		[HttpPut("{id}")]
		[ProducesResponseType(typeof(CandidateResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateCandidateRequest request, CancellationToken ct = default)
		{
			return Ok(await _candidateService.UpdateAsync(id, request, ct));
		}

		/// <summary>Exclui um candidato</summary>
		/// <param name="id">O ID do candidato a ser excluído</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>Sucesso</returns>
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken ct = default)
		{
			await _candidateService.DeleteAsync(id, ct);
			return NoContent();
		}
	}
}