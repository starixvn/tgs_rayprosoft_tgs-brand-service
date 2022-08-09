using System;
using System.Text.Json.Serialization;

namespace Tgs.BrandService.ViewModels
{
	public class BrandMaintenanceViewModel
	{
		public string Content { get; set; } = default!;

		[JsonConverter(typeof(TimeOnlyConverter))]
		public TimeOnly BeginTime { get; set; }

		[JsonConverter(typeof(TimeOnlyConverter))]
		public TimeOnly EndTime { get; set; }
	}
}
