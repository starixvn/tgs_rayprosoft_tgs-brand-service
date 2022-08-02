using System;

namespace Tgs.BrandService
{
	public class BrandMaintenanceInfo
	{
		public string Content { get; set; } = default!;

		public TimeOnly BeginTime { get; set; }

		public TimeOnly EndTime { get; set; }
	};
}
