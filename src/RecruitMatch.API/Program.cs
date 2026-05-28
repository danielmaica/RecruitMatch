using Microsoft.AspNetCore.Diagnostics;

using MongoDB.Driver;

using RecruitMatch.Application.Interfaces;
using RecruitMatch.Application.Services;
using RecruitMatch.Domain.Interfaces;
using RecruitMatch.Infrastructure.AI;
using RecruitMatch.Infrastructure.Persistence;
using RecruitMatch.Infrastructure.Persistence.Repositories;

using Scalar.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddOpenApi();

// Buscar configurações do appsettings.json
var groqSettings = builder.Configuration.GetSection("GroqSettings").Get<GroqSettings>() ?? throw new Exception("Configuração do Groq não encontrada.");

var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>() ?? throw new Exception("Configuração do MongoDB não encontrada.");

// Injetar GroqSettings para uso em serviços
builder.Services.Configure<GroqSettings>(builder.Configuration.GetSection("GroqSettings"));

// MongoDB setup
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(mongoSettings.DatabaseName));


// Repositories
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();

// Services
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IAIMatchService, AIMatchService>();

// Http Factory
builder.Services.AddHttpClient("groq", client =>
{
	client.BaseAddress = new Uri(groqSettings.Uri);
	client.DefaultRequestHeaders.Add("Authorization", $"Bearer {groqSettings.ApiKey}");
});

// Mapper
BsonMappings.Register();

var app = builder.Build();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
	var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;

	var (status, message) = error switch
	{
		KeyNotFoundException e => (StatusCodes.Status404NotFound, e.Message),
		ArgumentException e => (StatusCodes.Status400BadRequest, e.Message),
		_ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor."),
	};

	context.Response.StatusCode = status;
	context.Response.ContentType = "application/json";
	await context.Response.WriteAsJsonAsync(new { error = message });
}));

app.MapOpenApi();
app.MapScalarApiReference();


app.UseHttpsRedirection();

app.MapControllers();
app.Run();

