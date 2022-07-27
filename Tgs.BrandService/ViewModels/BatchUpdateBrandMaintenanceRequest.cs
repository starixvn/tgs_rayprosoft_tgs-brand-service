using System;

namespace Tgs.BrandService.ViewModels
{
	public record BatchUpdateBrandMaintenanceRequest
	{
		public Guid[] BrandIds { get; init; } = default!;
		public DateTimeOffset BeginTime { get; init; }
		public DateTimeOffset EndTime { get; init; }
		public string Content { get; init; } = default!;
	}
}
