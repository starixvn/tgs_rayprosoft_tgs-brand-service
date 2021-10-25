using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Tgs.BrandService
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services
				.AddControllers()
				.AddMvcOptions(options =>
				{
					options.AddWrapResponseFilters();
					options.AddWebApiToolkitFilters();
				});

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

				var xml = $"Tgs.BrandService.xml";
				var path = Path.Combine(AppContext.BaseDirectory, xml);
				if (File.Exists(path))
					options.IncludeXmlComments(path);

			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwaggerUI(options =>
			{
				options.SwaggerEndpoint("/swagger/v1/swagger.yaml", "Tgs Brand Api");
			});

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapHealthChecks("/healthz");
				endpoints.MapSwagger();
			});
		}
	}
}
