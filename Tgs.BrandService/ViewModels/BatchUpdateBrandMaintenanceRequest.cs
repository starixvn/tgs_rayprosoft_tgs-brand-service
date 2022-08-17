using System;

namespace Tgs.BrandService.ViewModels
{
	public record BatchUpdateBrandMaintenanceRequest
	{
		public Guid[] BrandIds { get; init; } = default!;
		public TimeOnly BeginTime { get; init; }
		public TimeOnly EndTime { get; init; }
		public string Content { get; init; } = default!;
	}
}
