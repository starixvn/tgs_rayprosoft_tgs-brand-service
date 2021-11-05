using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.ApplicationServices
{
	internal class BrandLiveChatQueryService : IGetService<BrandLiveChatQuery, BrandLiveChatInfo?>
	{
		private readonly ITgsRepository _tgsRepository;

		public BrandLiveChatQueryService(ITgsRepository tgsRepository)
		{
			_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
		}

		public ValueTask<BrandLiveChatInfo?> GetAsync(
			BrandLiveChatQuery query,
			CancellationToken cancellationToken = default)
			=> _tgsRepository.GetBrandLiveChatInfoAsync(query.BrandId);
	}
}
