using System.Text.Json.Serialization;

namespace Tgs.BrandService.ViewModels
{
	public class BrandMaintenanceViewModel
	{
		[JsonPropertyName("content")]
		public string Content { get; init; } = default!;
	}
}
