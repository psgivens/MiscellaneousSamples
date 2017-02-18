using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Autofac.Integration.Wcf;
using PhillipScottGivens.WebDownloader.Services;
using Autofac;
using PhillipScottGivens.Common;
using System.Messaging;
using System.ServiceModel.Description;

namespace PhillipScottGivens.WebDownloader.ServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // Glue together the system.
            ContainerBuilder builder = new ContainerBuilder();

            // Singletons
            builder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<EventBus>().As<IEventBus>().SingleInstance();

            // Per request.
            builder.RegisterType<CachingService>().As<ICachingService>();
            builder.RegisterType<DownloadService>().As<IDownloadService>();
            builder.RegisterType<DownloaderClient>().As<IDownloaderClient>();
            builder.RegisterType<DownloadItemRepository>().As<IDownloadItemRepository>();
            
            // Get the relevant parts. 
            IContainer container = builder.Build();
            ILogger logger = container.Resolve<ILogger>();
            ICachingService cachingService = container.Resolve<ICachingService>();
            IEventBus eventBus = container.Resolve<IEventBus>();
            Func<IDownloadItemRepository> createDownloadRepository = container.Resolve<Func<IDownloadItemRepository>>();

            ServiceHost queueHost = new ServiceHost(typeof(DownloadService));
            if (!MessageQueue.Exists(@".\private$\DownloadService"))
            {
                MessageQueue.Create(@".\private$\DownloadService", true);
            }
            queueHost.AddDependencyInjectionBehavior<IDownloadService>(container);
            queueHost.Open();

            ServiceHost host = new ServiceHost(typeof(CachingService));
            host.AddDependencyInjectionBehavior<ICachingService>(container);
            host.Description.Behaviors.Add(new ServiceMetadataBehavior { HttpGetEnabled = true, HttpGetUrl = new Uri("http://localhost:8080/WebDownloaderService") });
            host.Open();

            Console.WriteLine("The host has been opened.");
            Console.ReadLine();

            host.Close();
            queueHost.Close();
            Environment.Exit(0);
        }
    }
}
