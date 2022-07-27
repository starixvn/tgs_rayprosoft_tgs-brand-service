using System;

namespace Tgs.BrandService.ApplicationServices
{
	public record BatchUpdateBrandMaintenanceCommand
	{
		public Guid[] BrandIds { get; set; } = default!;
		public DateTimeOffset BeginTime { get; set; }
		public DateTimeOffset EndTime { get; set; }
		public string Content { get; set; } = default!;
	}
}
