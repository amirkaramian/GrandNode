using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Cms;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipping.SendCloud.Services;

namespace Shipping.SendCloud
{
    public class StartupApplication : IStartupApplication
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IShippingSendCloudService, ShippingSendCloudService>();
            services.AddScoped<IShippingRateCalculationProvider, SendCloudShippingCalcPlugin>();
            services.AddScoped<IWidgetProvider, SendCloudProvider>();
            services.AddScoped<IConsentCookie, SendCloudConsentCookie>();
        }

        public int Priority => 10;
        public void Configure(IApplicationBuilder application, IWebHostEnvironment webHostEnvironment)
        {

        }
        public bool BeforeConfigure => false;

    }
}
