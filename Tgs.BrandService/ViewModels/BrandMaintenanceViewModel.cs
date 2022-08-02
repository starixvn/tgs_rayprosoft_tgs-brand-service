using System;
using System.Text.Json.Serialization;

namespace Tgs.BrandService.ViewModels
{
	public class BrandMaintenanceViewModel
	{
		public string Content { get; set; } = default!;

		
		public TimeOnly BeginTime { get; set; }

		
		public TimeOnly EndTime { get; set; }
	}
}
