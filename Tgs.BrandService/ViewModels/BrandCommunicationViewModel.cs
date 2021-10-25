using System.Text.Json.Serialization;

namespace Tgs.BrandService.ViewModels
{
	public class BrandCommunicationViewModel
	{
		public string Type { get; init; } = default!;

		public string Phone { get; init; } = default!;

		[JsonPropertyName("appid")]
		public string AppId { get; init; } = default!;

		[JsonPropertyName("qrcodeurl")]
		public string QRCodeUrl { get; init; } = default!;
	}
}
