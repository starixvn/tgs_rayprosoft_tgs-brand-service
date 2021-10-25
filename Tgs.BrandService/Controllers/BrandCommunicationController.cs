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
		public IAsyncEnumerable<BrandCommunicationViewModel> GetBrandCommunicationsAsync()
		{
			return new[]{
				new BrandCommunicationViewModel{
					AppId = "test",
					Phone ="123456789",
					QRCodeUrl = "https://test.com/1.png",
					Type = "type"
				}
				}.ToAsyncEnumerable();
		}
	}
}
