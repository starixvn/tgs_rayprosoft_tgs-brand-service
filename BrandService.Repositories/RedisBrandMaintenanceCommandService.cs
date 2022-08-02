using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.BrandService.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Repositories
{
	internal class RedisBrandMaintenanceCommandService : ICommandService<BatchUpdateBrandMaintenanceCommand>
	{
		private readonly StackExchange.Redis.IDatabase _redisDatabase;
		private readonly ICommandService<BatchUpdateBrandMaintenanceCommand> _targetService;
		private readonly ILogger<RedisBrandMaintenanceCommandService> _logger;

		public RedisBrandMaintenanceCommandService(
			ICommandService<BatchUpdateBrandMaintenanceCommand> targetService,
			RedisConnectionFactory redisConnectionFactory,
			ILogger<RedisBrandMaintenanceCommandService> logger)
		{
			if (redisConnectionFactory is null)
				throw new ArgumentNullException(nameof(redisConnectionFactory));

			_redisDatabase = redisConnectionFactory.Database;
			_targetService = targetService;
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async ValueTask ExecuteAsync(BatchUpdateBrandMaintenanceCommand command, CancellationToken cancellationToken = default)
		{
			foreach (var brandId in command.BrandIds)
			{
				var key = $"Tgs:Maintenance:{brandId}";

				var data = new BrandMaintenanceInfo
				{
					Content = command.Content,
					BeginTime = command.BeginTime,
					EndTime = command.EndTime
				};

				var options = new JsonSerializerOptions();
				options.Converters.Add(new TimeOnlyConverter());

				_ = await _redisDatabase
					.StringSetAsync(
						key,
						data == null
							? string.Empty
							: JsonSerializer.Serialize(data, options),
						flags: CommandFlags.FireAndForget)
					.ConfigureAwait(false);
			}

			_logger.LogInformation("Batch Update Maintenance Redis Success");
		}
	}
}
