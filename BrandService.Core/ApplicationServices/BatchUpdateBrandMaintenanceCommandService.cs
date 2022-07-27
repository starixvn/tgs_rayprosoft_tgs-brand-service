using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.ApplicationServices
{
	internal class BatchUpdateBrandMaintenanceCommandService : ICommandService<BatchUpdateBrandMaintenanceCommand>
	{
		public ValueTask ExecuteAsync(BatchUpdateBrandMaintenanceCommand command, CancellationToken cancellationToken = default)
		{
			return ValueTask.CompletedTask;
		}
	}
}
