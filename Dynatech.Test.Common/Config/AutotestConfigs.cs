using Dynatech.Test.Common.EnumType;
using Microsoft.Extensions.Configuration;

namespace Dynatech.Test.Common.Config {
    public static class AutotestConfigs {
        private static readonly string solutionPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
        public static string ChromeDriverUrl { get; }
        public static string TestUrl { get; }
        public static bool EnableHeadless { get; }
        public static MonitorPosition MonitorPosition { get; }
        
        static AutotestConfigs() {
            // check global Environment
            var configFileName = "";
            var testConfigFile = Environment.GetEnvironmentVariable("TEST_CONFIG_FILE");

            if (testConfigFile != null) {
                configFileName = testConfigFile;
            }
            else {
                // look flag on local file
                var devConfigFlagPath = Path.Combine(solutionPath, "Dynatech.Test.Common", "Config", ".Dev.Config.Flag.conf");
                configFileName = File.ReadAllLines(devConfigFlagPath)[0];
            }
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(solutionPath, "Dynatech.Test.Common", "Config"))
                .AddJsonFile(configFileName, false, false)
                .Build();

            ChromeDriverUrl = configuration.GetSection("CHROME_DRIVER_URL").Value;
            TestUrl = configuration.GetSection("TEST_URL").Value;
            EnableHeadless = bool.Parse(configuration.GetSection("ENABLE_HEADLESS").Value);

            MonitorPosition = configuration.GetSection("MONITOR_POSITION").Value switch {
                "Left" => MonitorPosition.LEFT,
                "Center" => MonitorPosition.CENTER,
                "Right" => MonitorPosition.RIGHT,
                _ => MonitorPosition
            };
        }
    }
}

