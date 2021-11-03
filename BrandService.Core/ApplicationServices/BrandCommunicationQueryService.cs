using System;
using System.Collections.Generic;
using System.Threading;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.ApplicationServices
{
	internal class BrandCommunicationQueryService : IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>
	{
		private readonly ITgsRepository _tgsRepository;

		public BrandCommunicationQueryService(ITgsRepository tgsRepository)
		{
			_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
		}

		public IAsyncEnumerable<BrandCommunicationInfo> QueryAsync(
			BrandCommunicationQuery query,
			CancellationToken cancellationToken = default)
			=> _tgsRepository.GetBrandCommunicationsAsync(query.BrandId);
	}
}
