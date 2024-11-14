using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tgs.BrandService;
using Tgs.BrandService.ApplicationServices;
using Tgs.BrandService.MessageQueue;
using Tgs.BrandService.Models;
using Tgs.ServiceUtility;

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddCors()
	.AddOptions<CorsOptions>()
	.Configure<IConfiguration>((options, config) =>
	{
		var urls = config.GetSection("CorsDomain:Urls")
			.GetChildren()
			.Select(t => t.Get<string>())
			.ToArray();

		options.AddDefaultPolicy(
			b =>
			{
				b.WithOrigins(urls!)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.SetIsOriginAllowed(_ => true);
			});
	});

builder.Services
	.AddBrandCertificateClient((options, sp) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();

		options.BrandCertificateBaseUrl = config.GetValue<Uri>("Brand:BrandCertificateBaseUrl")!;
	})
	.AddMemoryCache()
	.AddBrandService()
	.AddBrandServiceRepositories((sp, b) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();

		b.UseNpgsql(config.GetConnectionString("Tgs")!);
	})
	.DecorateRedisBrandCommunicationQueryService((options, sp) =>
	{
		var config = sp.GetRequiredService<IConfiguration>();

		options.ConnectionString = config.GetConnectionString("Redis")!;
	})
	.Decorate<IQueryService<BrandCommunicationQuery, BrandCommunicationInfo>, CachedBrandCommunicationQueryService>()
	.Decorate<IGetService<BrandLiveChatQuery, BrandLiveChatInfo?>, CachedBrandLiveChatQueryService>()
	.Decorate<IGetService<BrandMaintenanceQuery, BrandMaintenanceInfo?>, CachedBrandMaintenanceQueryService>();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services
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

builder.Services
	.AddMessageQueue()
	.AddNatsMessageQueue(natsConfig =>
	{
		natsConfig
			.ConfigQueueOptions((options, sp) =>
			{
				var config = sp.GetRequiredService<IConfiguration>();

				options.Url = config.GetConnectionString("Nats")!;
			})
			.AddHandler(ProcessorTypes.ClearMaintenanceMemoryCacheHandlerType,
				"tgs.brandservice.maintenance.cache.clear",
				"tgs.brandservice.maintenance.cache.clear");
	})
	.AddNatsGlobPatternExchange("*");

builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();
app.UseBrandCertificate();
app.UseSwaggerUI();
app.UseRouting();

app.MapControllers();
app.MapSwagger();
app.MapHealthChecks("/healthz");

app.Run();