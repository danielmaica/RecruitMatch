using System.Text;
using System.Text.Json;

using Microsoft.Extensions.Options;

using RecruitMatch.Application.DTOs.Responses;
using RecruitMatch.Application.Interfaces;
using RecruitMatch.Domain.Entities;

namespace RecruitMatch.Infrastructure.AI;

public class GroqMatchService : IAIMatchService
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly GroqSettings _settings;

	public GroqMatchService(IHttpClientFactory httpClientFactory, IOptions<GroqSettings> settings)
	{
		_httpClientFactory = httpClientFactory;
		_settings = settings.Value;
	}

	public async Task<MatchAIResult> AnalyzeAsync(Job job, Candidate candidate, CancellationToken ct = default)
	{
		var prompt = PromptTemplates.MatchAnalysis(job, candidate);

		var httpClient = _httpClientFactory.CreateClient();

		var body = new
		{
			model = _settings.Model,
			messages = new[]
			{
				new
				{
					role = "system",
					content = "Você é um especialista em recrutamento e seleção, responsável por analisar a compatibilidade entre candidatos e vagas de emprego com base em seus currículos e descrições de vagas."
				},
				new
				{
					role = "user",
					content = prompt
				}
			}
		};

		httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");

		var response = await httpClient.PostAsync(_settings.Uri, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"), ct);
		if (!response.IsSuccessStatusCode)
		{
			var errorContent = await response.Content.ReadAsStringAsync(ct);
			throw new Exception($"Erro na chamada à API de IA: {response.StatusCode}, {errorContent}");
		}

		var content = await response.Content.ReadAsStringAsync(ct);

		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		};
		var groqResponse = JsonSerializer.Deserialize<GroqResponse>(content, options);
		var modelOutput = groqResponse!.Choices[0].Message.Content;

		return JsonSerializer.Deserialize<MatchAIResult>(modelOutput, options)!;
	}

	private record GroqResponse(List<GroqChoice> Choices);

	private record GroqChoice(GroqMessage Message);
	private record GroqMessage(string Content);
}

