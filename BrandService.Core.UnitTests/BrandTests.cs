using System;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Tgs.BrandService;
using Tgs.BrandService.ApplicationServices;

namespace BrandService.Core.UnitTests
{
	[TestFixture]
	public class BrandTests
	{
		[Test]
		public async Task Can_Get_BrandCommunication()
		{
			var fakeTgsRepository = Substitute.For<ITgsRepository>();

			var sut = new BrandCommunicationQueryService(fakeTgsRepository);

			var query = new BrandCommunicationQuery(Guid.NewGuid());

			await sut.QueryAsync(query).ToArrayAsync();

			_ = fakeTgsRepository.Received(1)
				.GetBrandCommunicationsAsync(Arg.Is(query.BrandId));
		}

		[Test]
		public async Task Can_Get_BrandLiveChat()
		{
			var fakeTgsRepository = Substitute.For<ITgsRepository>();

			var sut = new BrandLiveChatQueryService(fakeTgsRepository);

			var query = new BrandLiveChatQuery(Guid.NewGuid());

			await sut.GetAsync(query);

			_ = fakeTgsRepository.Received(1)
				.GetBrandLiveChatInfoAsync(query.BrandId);
		}

		[Test]
		public async Task Can_Get_BrandMaintenance()
		{
			var fakeTgsRepository = Substitute.For<ITgsRepository>();

			var sut = new BrandMaintenanceQueryService(fakeTgsRepository);

			var query = new BrandMaintenanceQuery(Guid.NewGuid());

			await sut.GetAsync(query);

			_ = fakeTgsRepository.Received(1)
				.GetBrandMaintenanceInfoAsync(query.BrandId);
		}
	}
}
