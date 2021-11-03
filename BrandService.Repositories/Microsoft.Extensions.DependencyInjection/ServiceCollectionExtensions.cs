using System;
using Microsoft.EntityFrameworkCore;
using Tgs.BrandService;
using Tgs.BrandService.ApplicationServices;
using Tgs.BrandService.Repositories;
using Tgs.BrandService.Repositories.Entities;
using Tgs.ServiceUtility;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBrandServiceRepositories(
			this IServiceCollection services,
			Action<IServiceProvider, DbContextOptionsBuilder> dbContextConfigure)
			=> services
				.AddDbContext<TgsContext>(dbContextConfigure)
				.AddTransient<ITgsRepository, TgsRepository>();

		public static IServiceCollection DecorateRedisBrandCommunicationQueryService(
			this IServiceCollection services,
			Action<RedisOptions, IServiceProvider> optionsConfigure)
			=> services
				.AddOptions<RedisOptions>()
				.Configure(optionsConfigure)
				.Services
				.AddSingleton<RedisConnectionFactory>()
				.Decorate<IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>, RedisBrandCommunicationQueryService>();
	}
}
