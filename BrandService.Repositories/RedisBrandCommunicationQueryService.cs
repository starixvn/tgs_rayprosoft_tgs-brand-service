using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;
using StackExchange.Redis;
using Tgs.BrandService.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Repositories
{
	internal class RedisBrandCommunicationQueryService : IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>
	{
		private readonly IDatabase _redisDatabase;
		private readonly IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> _targetService;
		private readonly ILogger<RedisBrandCommunicationQueryService> _logger;

		public RedisBrandCommunicationQueryService(
			IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> targetService,
			RedisConnectionFactory redisConnectionFactory,
			ILogger<RedisBrandCommunicationQueryService> logger)
		{
			if (redisConnectionFactory is null)
				throw new ArgumentNullException(nameof(redisConnectionFactory));

			_redisDatabase = redisConnectionFactory.Database;
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async IAsyncEnumerable<BrandCommunicationInfo> QueryAsync(
			BrandCommunicationQuery query,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			var data = await Policy<BrandCommunicationInfo[]>
				.Handle<Exception>()
				.FallbackAsync(
					async _ =>
					{
						var key = $"Tgs:Communication:{query.BrandId}";

						var value = await _redisDatabase
							.StringGetAsync(key)
							.ConfigureAwait(false);

						if (value.HasValue)
							return JsonSerializer.Deserialize<BrandCommunicationInfo[]>(value);
						else
							return Array.Empty<BrandCommunicationInfo>();
					})
				.WrapAsync(
					Policy.TimeoutAsync(
						1,
						TimeoutStrategy.Pessimistic,
						(context, timespan, task) =>
						{
							_logger.LogInformation("Database get data timeout.");

							return Task.CompletedTask;
						}))
				.ExecuteAsync(
					() => Task.Run(async () =>
					{
						var data = await _targetService
							.QueryAsync(query)
							.ToArrayAsync()
							.ConfigureAwait(false);

						var key = $"Tgs:Communication:{query.BrandId}";

						await _redisDatabase
							.StringSetAsync(
								key,
								JsonSerializer.Serialize(data),
								flags: CommandFlags.FireAndForget)
							.ConfigureAwait(false);

						return data;
					}))
				.ConfigureAwait(false);

			foreach (var item in data)
			{
				cancellationToken.ThrowIfCancellationRequested();

				yield return item;
			}
		}
	}
}
