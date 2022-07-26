using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Tgs.BrandService.Protos;
using Tgs.MessageQueue;

namespace Tgs.BrandService.MessageQueue
{
	internal class ClearMaintenanceMemoryCacheHandler : IMessageHandler
	{
		private readonly IMemoryCache _memoryCache;
		private readonly ILogger<ClearMaintenanceMemoryCacheHandler> _logger;

		public ClearMaintenanceMemoryCacheHandler(
			IMemoryCache memoryCache,
			ILogger<ClearMaintenanceMemoryCacheHandler> logger)
		{
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
		{
			var request = ClearMaintenanceCacheRequest.Parser.ParseFrom(data.Span);

			var key = $"Maintenance:{request.BrandId}";

			_memoryCache.Remove(key);

			_logger.LogInformation("Maintenance Memory Cache Remove");

			return ValueTask.CompletedTask;
		}
	}
}
