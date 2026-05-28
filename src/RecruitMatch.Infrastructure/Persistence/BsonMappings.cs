using MongoDB.Bson.Serialization;

using RecruitMatch.Domain.Entities;
using RecruitMatch.Domain.ValueObjects;

namespace RecruitMatch.Infrastructure.Persistence
{
	public static class BsonMappings
	{
		public static void Register()
		{
			// Entities
			BsonClassMap.RegisterClassMap<Entity>(cm =>
			{
				cm.AutoMap();
				cm.MapIdProperty(e => e.Id);
			});

			BsonClassMap.RegisterClassMap<AggregateRoot>(cm => cm.AutoMap());

			BsonClassMap.RegisterClassMap<Candidate>(cm =>
			{
				cm.AutoMap();
				cm.MapConstructor(
			typeof(Candidate).GetConstructors().First(),
			nameof(Candidate.Name), nameof(Candidate.Email), nameof(Candidate.Resume), nameof(Candidate.Skills)
	);
			});

			BsonClassMap.RegisterClassMap<Job>(cm =>
			{
				cm.AutoMap();
				cm.MapConstructor(
			typeof(Job).GetConstructors().First(),
			nameof(Job.Title), nameof(Job.Description), nameof(Job.Seniority), nameof(Job.Requirements)
	);
			});

			BsonClassMap.RegisterClassMap<Match>(cm =>
			{
				cm.AutoMap();
				cm.MapConstructor(
			typeof(Match).GetConstructors().First(),
			nameof(Match.JobId), nameof(Match.CandidateId), nameof(Match.CandidateName),
			nameof(Match.Score), nameof(Match.ResumeSummary), nameof(Match.Justification),
			nameof(Match.Strengths), nameof(Match.Gaps)
	);
			});


			// Value objects
			BsonClassMap.RegisterClassMap<JobRequirements>(cm =>
			{
				cm.AutoMap();
				cm.MapConstructor(
					typeof(JobRequirements).GetConstructors().First(),
					nameof(JobRequirements.Required),
					nameof(JobRequirements.Preferred)
				);
			});
			BsonClassMap.RegisterClassMap<Email>(cm =>
			{
				cm.AutoMap();
				cm.MapCreator(e => new Email(e.Address));
			});
			BsonClassMap.RegisterClassMap<MatchScore>(cm =>
			{
				cm.AutoMap();
				cm.MapCreator(ms => new MatchScore(ms.Value));
			});
		}
	}
}