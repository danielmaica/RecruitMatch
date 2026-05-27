using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;
using RecruitMatch.Domain.Entities;

namespace RecruitMatch.Infrastructure.AI;

public class GroqMatchService : IAIMatchService
{
	private readonly ILogger<GroqMatchService> _logger;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly GroqSettings _settings;

	public GroqMatchService(ILogger<GroqMatchService> logger, IHttpClientFactory httpClientFactory, IOptions<GroqSettings> settings)
	{
		_logger = logger;
		_httpClientFactory = httpClientFactory;
		_settings = settings.Value;
	}

	public async Task<MatchAIResult> AnalyzeAsync(Job job, Candidate candidate, CancellationToken ct = default)
	{
		var systemPrompt = PromptTemplates.SystemPrompt();
		var userPrompt = PromptTemplates.MatchAnalysisPrompt(job, candidate);

		var httpClient = _httpClientFactory.CreateClient();

		var body = new
		{
			model = _settings.Model,
			messages = new[]
			{
				new
				{
					role = "system",
					content = systemPrompt
				},
				new
				{
					role = "user",
					content = userPrompt
				}
			}
		};

		httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

		var response = await httpClient.PostAsync(_settings.Uri,
			new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"), ct);
		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync(ct);
			throw new Exception($"Erro na chamada à API de IA: {response.StatusCode}, {errorContent}");
		}

		var content = await response.Content.ReadAsStringAsync(ct);
		_logger.LogDebug("Resposta da API Groq: {Content}", content);

		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		};
		var groqResponse = JsonSerializer.Deserialize<GroqResponse>(content, options);
		if (groqResponse?.Choices is null || groqResponse.Choices.Count == 0)
			throw new InvalidOperationException($"Resposta inválida da API Groq: sem choices. Conteúdo: {content}");

		var modelOutput = groqResponse!.Choices[0].Message.Content
			.Replace("```json", "")
			.Replace("```", "")
			.Trim();

		return JsonSerializer.Deserialize<MatchAIResult>(modelOutput, options)!;
	}

	private record GroqResponse(List<GroqChoice> Choices);

	private record GroqChoice(GroqMessage Message);

	private record GroqMessage(string Content);
}