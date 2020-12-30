using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace GEP.SMART.Requisition.API
{

    [ExcludeFromCodeCoverage]
    public class Program
    {
        private static IConfiguration _configuration;
        public static void Main(string[] args)
        {
            string certificateThumbPrint = string.Empty;
            var host = new WebHostBuilder()
                .UseKestrel(o =>
                {
                    o.Listen(IPAddress.Any, 80);
                    o.Listen(IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.NoDelay = false;
                        // Enable https
                        listenOptions.UseHttps(new X509Certificate2(GetCertificateFromStore(certificateThumbPrint)));
                    });
                })
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    _configuration = config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();
                    certificateThumbPrint = _configuration.GetValue<string>("GEPWildcardCertThumbPrint");
                    config.AddEnvironmentVariables();
                })
                .UseUrls("https://*:443")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();
            host.Run();
        }
        private static X509Certificate2 GetCertificateFromStore(string certificateThumbPrint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var certCollection = store.Certificates;
                var currentCerts = certCollection.Find(X509FindType.FindByThumbprint, certificateThumbPrint, false);
                return currentCerts.Count == 0 ? null : currentCerts[0];
            }
            finally
            {
                store.Close();
            }
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

}
