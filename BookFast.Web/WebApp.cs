﻿using System.Collections.Generic;
using System.Fabric;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace BookFast.Web
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class WebApp : StatelessService
    {
        public WebApp(StatelessServiceContext context)
            : base(context)
        { }
        
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", url =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting WebListener on {url}");

                        return new WebHostBuilder().UseKestrel()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }
    }
}