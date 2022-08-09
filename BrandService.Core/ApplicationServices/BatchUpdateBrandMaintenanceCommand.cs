using System;

namespace Tgs.BrandService.ApplicationServices
{
	public record BatchUpdateBrandMaintenanceCommand
	{
		public Guid[] BrandIds { get; set; } = default!;
		public TimeOnly BeginTime { get; set; }
		public TimeOnly EndTime { get; set; }
		public string Content { get; set; } = default!;
	}
}
