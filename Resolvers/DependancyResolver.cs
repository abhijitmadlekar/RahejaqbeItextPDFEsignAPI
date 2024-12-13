using DocEsignAPI.Domain.Services;
using DocEsignAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocEsignAPI.Resolvers
{
    public class DependancyResolver
    {
        public static void Register(IServiceCollection services)
        {
            services.AddScoped<EsignService>();
        }
    }
}
