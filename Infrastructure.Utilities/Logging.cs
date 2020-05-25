using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Infrastructure.Utilities
{
    public class Logging
    {
        private static readonly string LogRoot = HostingEnvironment.IsHosted ? HostingEnvironment.ApplicationPhysicalPath : Environment.CurrentDirectory;

        public static void Information(string msg, string folder = "")
        {
            string logFileName = DateTime.Now.ToString("yyy-MM-dd") + ".log";
            string path = Path.Combine(LogRoot, "Logs\\" + folder + "\\" + logFileName);

            if (!File.Exists(path))
                File.Create(path).Dispose();
            
            Log.Logger = new LoggerConfiguration().WriteTo.Console()
                .WriteTo.File(path)
                .CreateLogger();

            Log.Information(msg);
            Log.CloseAndFlush();
            
        }
    }
}
