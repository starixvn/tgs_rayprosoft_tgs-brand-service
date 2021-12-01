using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.BrandService.ViewModels
{
	public class BrandLiveChatViewModel
	{
		[JsonPropertyName("name")]
		public string Name { get; init; } = default!;

		[JsonPropertyName("setting")]
		public JsonDocument Setting { get; init; } = default!;
	}
}
