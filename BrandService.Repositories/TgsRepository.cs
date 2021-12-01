using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tgs.BrandService.Repositories.Entities;

namespace Tgs.BrandService.Repositories
{
	internal class TgsRepository : ITgsRepository
	{
		private readonly TgsContext _dbContext;

		public TgsRepository(TgsContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}

		public IAsyncEnumerable<BrandCommunicationInfo> GetBrandCommunicationsAsync(Guid brandId)
			=> _dbContext.BrandCommunications.AsQueryable()
				.Where(row => row.BrandId == brandId)
				.Select(row => new BrandCommunicationInfo()
				{
					CommunicationType = row.Type!,
					Phone = row.Phone!,
					AppId = row.AppId!,
					QRCodeUrl = row.QrCodeUrl!
				})
				.Take(5)
				.ToAsyncEnumerable();

		public async ValueTask<BrandLiveChatInfo?> GetBrandLiveChatInfoAsync(Guid brandId)
		{
			var brandLiveChat = await _dbContext.BrandLiveChats
				.FirstOrDefaultAsync(x => x.BrandId == brandId)
				.ConfigureAwait(false);

			if (brandLiveChat == null)
				return null;

			return new BrandLiveChatInfo(brandLiveChat.ChatName!, brandLiveChat.ChatSetting);
		}
	}
}
