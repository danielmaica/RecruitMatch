using Microsoft.AspNetCore.Mvc;

using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;

namespace RecruitMatch.API.Controllers.Api.V1
{
	[ApiController]
	[Route("api/v1/matches")]
	[Produces("application/json")]
	public class MatchesController : ControllerBase
	{
		private readonly IMatchService _matchService;
		public MatchesController(IMatchService matchService)
		{
			_matchService = matchService;
		}

		/// <summary>Analisa uma vaga</summary>
		/// <param name="jobId">O ID da vaga para análise</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>A correspondência obtida</returns>
		[HttpPost("analyze/{jobId}")]
		[ProducesResponseType(typeof(IReadOnlyList<MatchResponse>), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> AnalyzeAsync([FromRoute] string jobId, CancellationToken ct = default)
		{
			var matches = await _matchService.AnalyzeAsync(jobId, ct);
			return Created($"/api/v1/matches/{jobId}", matches);
		}

		/// <summary>Obtém correspondências pelo ID da vaga</summary>
		/// <param name="jobId">O ID da vaga para análise</param>
		/// <param name="ct">Token de cancelamento</param>
		/// <returns>Lista de correspondências</returns>
		[HttpGet("{jobId}")]
		[ProducesResponseType(typeof(IReadOnlyList<MatchResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByJobIdAsync([FromRoute] string jobId, CancellationToken ct = default)
		{
			return Ok(await _matchService.GetByJobIdAsync(jobId, ct));
		}
	}
}