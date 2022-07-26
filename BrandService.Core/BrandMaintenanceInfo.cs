using System;

namespace Tgs.BrandService
{
	public record BrandMaintenanceInfo(
		string Content,
		DateTimeOffset BeginTime,
		DateTimeOffset EndTime);
}
