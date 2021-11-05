using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.BrandService.ApplicationServices;
using Tgs.BrandService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BrandController : ControllerBase
	{
		[HttpGet("~/api/brandcommunication")]
		[HttpGet("communication")]
		public IAsyncEnumerable<BrandCommunicationViewModel> GetBrandCommunicationsAsync(
			[FromServices] IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> queryService)
		{
			var brandId = HttpContext.GetBrandId();

			var query = new BrandCommunicationQuery(brandId!.Value);

			return queryService.QueryAsync(query)
				.Select(info => new BrandCommunicationViewModel
				{
					AppId = info.AppId,
					Phone = info.Phone,
					QRCodeUrl = info.QRCodeUrl,
					Type = info.CommunicationType
				});
		}

		[HttpGet("~/api/brandlivechat")]
		[HttpGet("livechat")]
		public async Task<BrandLiveChatViewModel> GetBrandLiveChatAsync(
			[FromServices] IGetService<BrandLiveChatQuery, BrandLiveChatInfo> queryService)
		{
			var brandId = HttpContext.GetBrandId();

			var query = new BrandLiveChatQuery(brandId!.Value);

			var info = await queryService.GetAsync(query).ConfigureAwait(false);

			return new BrandLiveChatViewModel()
			{
				Name = info.Name,
				Setting = string.IsNullOrEmpty(info.Setting)
					? JsonDocument.Parse("{}")
					: JsonDocument.Parse(info.Setting)
			};
		}
	}
}
