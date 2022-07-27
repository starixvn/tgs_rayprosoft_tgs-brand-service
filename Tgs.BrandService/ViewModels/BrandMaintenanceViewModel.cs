using System;
using System.Text.Json.Serialization;

namespace Tgs.BrandService.ViewModels
{
	public class BrandMaintenanceViewModel
	{
		[JsonPropertyName("content")]
		public string Content { get; init; } = default!;

		[JsonPropertyName("begintime")]
		public DateTimeOffset BeginTime { get; init; }
		[JsonPropertyName("endtime")]
		public DateTimeOffset EndTime { get; init; }
	}
}
