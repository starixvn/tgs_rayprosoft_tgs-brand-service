using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.ApplicationServices
{
	class BrandCommunicationQueryService : IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>
	{
		public IAsyncEnumerable<BrandCommunicationInfo> QueryAsync(BrandCommunicationQuery query, CancellationToken cancellationToken = default) => throw new NotImplementedException();
	}
}
