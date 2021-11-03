using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Tgs.BrandService.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Models
{
	internal class CachedBrandCommunicationQueryService : IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>
	{
		private readonly IMemoryCache _memoryCache;
		private readonly IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> _targetService;
		private readonly ConcurrentDictionary<Guid, SemaphoreSlim> _lockObjects = new();

		public CachedBrandCommunicationQueryService(
			IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> targetService,
			IMemoryCache memoryCache)
		{
			_targetService = targetService ?? throw new ArgumentNullException(nameof(targetService));
			_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		}

		public async IAsyncEnumerable<BrandCommunicationInfo> QueryAsync(
			BrandCommunicationQuery query,
			[EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			var key = $"Communication:{query.BrandId}";

			if (!_memoryCache.TryGetValue<BrandCommunicationInfo[]>(key, out var cachedData))
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
							.QueryAsync(query, cancellationToken)
							.ToArrayAsync()
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

			foreach (var item in cachedData!)
			{
				cancellationToken.ThrowIfCancellationRequested();

				yield return item;
			}
		}
	}
}
