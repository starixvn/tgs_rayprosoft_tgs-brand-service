using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Tgs.BrandService.ApplicationServices;
using Tgs.BrandService.MessageQueue;
using Tgs.BrandService.Models;
using Tgs.ServiceUtility;

namespace Tgs.BrandService
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddBrandCertificateClient((options, sp) =>
				{
					var config = sp.GetRequiredService<IConfiguration>();

					options.BrandCertificateBaseUrl = config.GetValue<Uri>("Brand:BrandCertificateBaseUrl");
				})
				.AddMemoryCache()
				.AddBrandService()
				.AddBrandServiceRepositories((sp, builder) =>
				{
					var config = sp.GetRequiredService<IConfiguration>();

					builder.UseNpgsql(config.GetConnectionString("Tgs"));
				})
				.DecorateRedisBrandCommunicationQueryService((options, sp) =>
				{
					var config = sp.GetRequiredService<IConfiguration>();

					options.ConnectionString = config.GetConnectionString("Redis");
				})
				.Decorate<IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>, CachedBrandCommunicationQueryService>()
				.Decorate<IGetService<BrandLiveChatQuery, BrandLiveChatInfo?>, CachedBrandLiveChatQueryService>()
				.Decorate<IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?>, CachedBrandMaintenanceQueryService>();

			services.AddRouting(options => options.LowercaseUrls = true);
			services
				.AddControllers()
				.AddMvcOptions(options =>
				{
					options.AddBrandCertificateFilters();
					options.AddWrapResponseFilters();
					options.AddWebApiToolkitFilters();
					options.UseDateOnlyTimeOnlyStringConverters();
				})
				.AddJsonOptions(options =>
				{
					options.UseDateOnlyTimeOnlyStringConverters();
				});

			services
			.AddMessageQueue()
			.AddNatsMessageQueue(natsConfig =>
			{
				natsConfig
					.ConfigQueueOptions((options, sp) =>
					{
						var config = sp.GetRequiredService<IConfiguration>();

						options.Url = config.GetConnectionString("Nats");
					})
					.AddHandler(ProcessorTypes.ClearMaintenanceMemoryCacheHandlerType,
						"tgs.brandservice.maintenance.cache.clear",
						 "tgs.brandservice.maintenance.cache.clear");
			})
			.AddNatsGlobPatternExchange("*");

			services.AddHealthChecks();
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc(
					"v1",
					new OpenApiInfo
					{
						Title = "Tgs Brand Api",
						Version = "v1"
					});

				options.ApplyTgsWebApiSupport();
				options.AddBrandCertificateSecurityDefinition();

				var xml = $"Tgs.BrandService.xml";
				var path = Path.Combine(AppContext.BaseDirectory, xml);
				if (File.Exists(path))
					options.IncludeXmlComments(path);

				options.UseDateOnlyTimeOnlyStringConverters();
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseCors(x =>
			x.AllowAnyHeader()
			.AllowAnyHeader()
			.SetIsOriginAllowed(origin => true)
			.AllowCredentials());

			app.UseBrandCertificate();
			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Tgs Brand Api");
			});

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHealthChecks("/");
				endpoints.MapHealthChecks("/healthz");
				endpoints.MapSwagger();
			});
		}
	}
}
