using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddBrandService(this IServiceCollection services)
			=> services
				.RegisterAllServiceUtilityTypes(
					Assembly.GetExecutingAssembly(),
					ServiceLifetime.Transient);
	}
}
