using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DateOnlyTimeOnly.AspNet.Converters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
		private static readonly JsonSerializerOptions _SerializerOptions = new()
		{
			Converters = {
				new TimeOnlyJsonConverter()
			}
		};

		private readonly StackExchange.Redis.IDatabase _redisDatabase;
		private readonly IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?> _targetService;
		private readonly ILogger<RedisBrandMaintenanceQueryService> _logger;
		private readonly IHost _host;

		public RedisBrandMaintenanceQueryService(
			IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?> targetService,
			RedisConnectionFactory redisConnectionFactory,
			ILogger<RedisBrandMaintenanceQueryService> logger,
			IHost host)
		{
			if (redisConnectionFactory is null)
				throw new ArgumentNullException(nameof(redisConnectionFactory));

			_redisDatabase = redisConnectionFactory.Database;
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_host = host ?? throw new ArgumentNullException(nameof(host));
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
							return JsonSerializer.Deserialize<BrandMaintenanceInfo?>(value!, _SerializerOptions);
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
						await using var scope = _host.Services.CreateAsyncScope();

						var targetService =
							(IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?>)ActivatorUtilities.CreateInstance(scope.ServiceProvider, _targetService.GetType());

						var data = await targetService
								.GetAsync(query)
								.ConfigureAwait(false);

						var key = $"Tgs:Maintenance:{query.BrandId}";

						_ = await _redisDatabase
							.StringSetAsync(
								key,
								data == null
									? string.Empty
									: JsonSerializer.Serialize(data, _SerializerOptions),
								flags: CommandFlags.FireAndForget)
							.ConfigureAwait(false);

						return data;
					}))
				.ConfigureAwait(false);

			return data;
		}
	}
}
