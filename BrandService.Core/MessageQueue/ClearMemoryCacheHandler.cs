using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.BrandService.Protos;
using Tgs.MessageQueue;

namespace Tgs.BrandService.MessageQueue
{
	internal class ClearMaintenanceMemoryCacheHandler : IMessageHandler
	{
		private readonly IMemoryCache _memoryCache;

		public ClearMaintenanceMemoryCacheHandler(
			IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		}

		public ValueTask HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
		{
			var request = ClearMaintenanceMemoryCacheRequest.Parser.ParseFrom(data.Span);

			var key = $"Maintenance:{request.BrandId}";

			_memoryCache.Remove(key);

			return ValueTask.CompletedTask;
		}
	}
}
