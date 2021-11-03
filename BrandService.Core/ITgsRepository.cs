using System;
using System.Collections.Generic;

namespace Tgs.BrandService
{
	public interface ITgsRepository
	{
		IAsyncEnumerable<BrandCommunicationInfo> GetBrandCommunicationsAsync(Guid brandId);
	}
}
