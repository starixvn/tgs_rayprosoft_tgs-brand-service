using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tgs.BrandService
{
	public interface ITgsRepository
	{
		IAsyncEnumerable<BrandCommunicationInfo> GetBrandCommunicationsAsync(Guid brandId);

		ValueTask<BrandLiveChatInfo?> GetBrandLiveChatInfoAsync(Guid brandId);
	}
}
