using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tgs.BrandService.ApplicationServices;
using Tgs.BrandService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.BrandService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BrandCommunicationController : ControllerBase
	{
		[HttpGet]
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
	}
}
