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
	internal class RedisBrandMaintenanceQueryService : IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?>
	{
		private readonly StackExchange.Redis.IDatabase _redisDatabase;
		private readonly IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?> _targetService;
		private readonly ILogger<RedisBrandMaintenanceQueryService> _logger;

		public RedisBrandMaintenanceQueryService(
			IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?> targetService,
			RedisConnectionFactory redisConnectionFactory,
			ILogger<RedisBrandMaintenanceQueryService> logger)
		{
			if (redisConnectionFactory is null)
				throw new ArgumentNullException(nameof(redisConnectionFactory));

			_redisDatabase = redisConnectionFactory.Database;
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async ValueTask<BrandMaintenanceInfo?> GetAsync(BrandMaintenanceQuery query, CancellationToken cancellationToken = default)
		{
			var data = await Policy<BrandMaintenanceInfo?>
				.Handle<Exception>()
				.FallbackAsync(
					async _ =>
					{
						var key = $"Tgs:Maintenance:{query.BrandId}";

						var value = await _redisDatabase
							.StringGetAsync(key)
							.ConfigureAwait(false);

						if (value.HasValue && !string.IsNullOrEmpty(value))
							return JsonSerializer.Deserialize<BrandMaintenanceInfo?>(value);
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

						var key = $"Tgs:Maintenance:{query.BrandId}";


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
