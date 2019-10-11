﻿using Abyss.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using Abyss.Hosting;
using Abyss.Core.Services;
using System.Reflection;

namespace Abyss.Console
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            System.Console.WriteLine("Abyss console host application starting at " + DateTime.Now.ToString("F"));

            var contentRoot = args.Length > 0 ? args[0] : AppDomain.CurrentDomain.BaseDirectory;

            var host = new HostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureHostConfiguration(hostConfig =>
                {
                    hostConfig.SetBasePath(contentRoot);
                    hostConfig.AddJsonFile("hostsettings.json", optional: true);
                    hostConfig.AddEnvironmentVariables("Abyss_");
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    config.AddAbyssJsonFiles(hostingContext.HostingEnvironment.EnvironmentName);
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                })
                .ConfigureServices((hostBuildingContext, serviceCollection) =>
                {
                    // Configuration
                    var abyssConfig = new AbyssConfig();
                    hostBuildingContext.Configuration.Bind(abyssConfig);
                    serviceCollection.AddSingleton(abyssConfig);

                    // Application name
                    hostBuildingContext.HostingEnvironment.ApplicationName = abyssConfig.Name;

                    serviceCollection.ConfigureSharedServices();
                })
                .UseConsoleLifetime()
                .Build();

            var receiver = host.Services.GetRequiredService<MessageReceiver>();
            receiver.LoadTypesFromAssembly(Assembly.LoadFrom("Abyss.Commands.Default.dll"));

            host.Run();
        }
    }
}