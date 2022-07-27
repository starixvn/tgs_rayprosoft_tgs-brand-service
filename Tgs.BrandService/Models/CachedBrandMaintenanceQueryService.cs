using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tgs.BrandService.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Models
{
	public class CachedBrandMaintenanceQueryService : IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?>
	{
		private readonly IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?> _targetService;
		private readonly IMemoryCache _memoryCache;
		private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _lockObjects = new();

		public CachedBrandMaintenanceQueryService(
			IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?> targetService,
			IMemoryCache memoryCache)
		{
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		}

		public async ValueTask<BrandMaintenanceInfo?> GetAsync(BrandMaintenanceQuery query, CancellationToken cancellationToken = default)
		{
			var key = $"Maintenance:{query.BrandId}";

			if (!_memoryCache.TryGetValue<BrandMaintenanceInfo?>(key, out var cachedData))
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
