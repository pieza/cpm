using Microsoft.Extensions.Configuration;

namespace cpm.Utils
{
    static class Config
    {
        public static string HomeDir = Environment.GetEnvironmentVariable("HOME") ?? Environment.GetEnvironmentVariable("HOMEPATH") ?? "/";
        public static string CPMDir = Path.Combine(HomeDir, ".cpm");

        private static IConfiguration? Instance
        {
            get
            {
                if (File.Exists(Path.Combine(CPMDir, "cpmsettings.json")))
                {
                    return new ConfigurationBuilder()
                                .SetBasePath(CPMDir)
                                .AddJsonFile($"cpmsettings.json", optional: false, reloadOnChange: true)
                                .Build();
                }
                return null;
            }
        }

        public static string? GetProperty(string key)
        {
            return Instance?[key];
        }
    }
}
