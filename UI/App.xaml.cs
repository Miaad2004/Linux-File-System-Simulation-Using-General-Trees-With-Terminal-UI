using FileSystem;
using FileSystem.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Navigation;
using UI.Services;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {

        const bool allocateConsole = false;

        // config path
        private static string ConfigPath = "D:\\Uni\\Term_3\\DS\\Projs\\LinuxFileSystemSimulation\\FileSystem\\config.json";

        public IServiceProvider ServiceProvider { get; private set; }


        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            // Setup configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigPath, optional: false, reloadOnChange: true)
                .Build();

            // Setup dependency injection
            var services = new ServiceCollection();
            ServiceFactory.ConfigureServices(services, configuration);
            ServiceProvider = services.BuildServiceProvider();

            var dbContext = ServiceProvider.GetRequiredService<LinuxDbContext>();
            dbContext.Database.EnsureCreated();

            if (allocateConsole)
            {
                AllocConsole();
            }

            base.OnStartup(e);
        }

        protected override void OnLoadCompleted(NavigationEventArgs e)
        {
            ConsoleInterfaceService.Instance.OnInitialized();
            base.OnLoadCompleted(e);
        }

    }

}
