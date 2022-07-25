using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.ApplicationServices
{
	internal class BrandMaintenanceQueryService : IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?>
	{
		private readonly ITgsRepository _tgsRepository;

		public BrandMaintenanceQueryService(ITgsRepository tgsRepository)
		{
			_tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
		}

		public ValueTask<BrandMaintenanceInfo?> GetAsync(BrandMaintenanceQuery query, CancellationToken cancellationToken = default)
			=> _tgsRepository.GetBrandMaintenanceInfoAsync(query.BrandId);
	}
}
