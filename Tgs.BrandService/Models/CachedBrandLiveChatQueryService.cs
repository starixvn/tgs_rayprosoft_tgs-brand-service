using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.BrandService.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Models
{
	public class CachedBrandLiveChatQueryService : IGetService<BrandLiveChatQuery, BrandLiveChatInfo?>
	{
		private readonly IGetService<BrandLiveChatQuery, BrandLiveChatInfo?> _targetService;
		private readonly IMemoryCache _memoryCache;
		private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _lockObjects = new();

		public CachedBrandLiveChatQueryService(
			IGetService<BrandLiveChatQuery, BrandLiveChatInfo?> targetService,
			IMemoryCache memoryCache)
		{
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		}

		public async ValueTask<BrandLiveChatInfo?> GetAsync(BrandLiveChatQuery query, CancellationToken cancellationToken = default)
		{
			var key = $"LiveChat:{query.BrandId}";

			if (!_memoryCache.TryGetValue<BrandLiveChatInfo?>(key, out var cachedData))
			{
				var lockObject = _lockObjects.GetOrAdd(
					query.BrandId,
					_ => new SemaphoreSlim(1, 1));

				await lockObject.WaitAsync().ConfigureAwait(false);

				try
				{
					if (!_memoryCache.TryGetValue(key, out cachedData))
					{
						cachedData = await _targetService
							.GetAsync(query, cancellationToken)
							.ConfigureAwait(false);

						_memoryCache.Set(
							key,
							cachedData,
							TimeSpan.FromSeconds(10D));
					}
				}
				finally
				{
					lockObject.Release();
				}
			}

			return cachedData;
		}
	}
}
