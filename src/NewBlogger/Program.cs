using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace NewBlogger
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

                var a=0;

            host.Run();
        }
    }
}
