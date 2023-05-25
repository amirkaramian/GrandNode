using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipping.SendCloud
{
    public class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Plugin.Shipping.SendCloud.GetLabel",
                "Plugins/WidgetsSendCloudInfo/GetLabel",
                new { controller = "WidgetsSendCloudInfo", action = "GetLabel" }
           );   
        }


        public int Priority => 0;

    }
}
