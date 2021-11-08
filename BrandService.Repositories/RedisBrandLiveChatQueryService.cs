using System;
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
	internal class RedisBrandLiveChatQueryService : IGetService<BrandLiveChatQuery, BrandLiveChatInfo?>
	{
		private readonly StackExchange.Redis.IDatabase _redisDatabase;
		private readonly IGetService<BrandLiveChatQuery, BrandLiveChatInfo?> _targetService;
		private readonly ILogger<RedisBrandLiveChatQueryService> _logger;

		public RedisBrandLiveChatQueryService(
			IGetService<BrandLiveChatQuery, BrandLiveChatInfo?> targetService,
			RedisConnectionFactory redisConnectionFactory,
			ILogger<RedisBrandLiveChatQueryService> logger)
		{
			if (redisConnectionFactory is null)
				throw new ArgumentNullException(nameof(redisConnectionFactory));

			_redisDatabase = redisConnectionFactory.Database;
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async ValueTask<BrandLiveChatInfo?> GetAsync(BrandLiveChatQuery query, CancellationToken cancellationToken = default)
		{
			var data = await Policy<BrandLiveChatInfo?>
				.Handle<Exception>()
				.FallbackAsync(
					async _ =>
					{
						var key = $"Tgs:LiveChat:{query.BrandId}";

						var value = await _redisDatabase
							.StringGetAsync(key)
							.ConfigureAwait(false);

						if (value.HasValue && !string.IsNullOrEmpty(value))
							return JsonSerializer.Deserialize<BrandLiveChatInfo?>(value);
						else
							return null;
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
							.GetAsync(query)
							.ConfigureAwait(false);

						var key = $"Tgs:LiveChat:{query.BrandId}";


						await _redisDatabase
							.StringSetAsync(
								key,
								data == null
									? string.Empty
									: JsonSerializer.Serialize(data),
								flags: CommandFlags.FireAndForget)
							.ConfigureAwait(false);

						return data;
					}))
				.ConfigureAwait(false);

			return data;
		}
	}
}
