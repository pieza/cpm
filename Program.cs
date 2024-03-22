using CommandLine;
using cpm.CLI;
using cpm.CLI.Handlers;
using cpm.Utils;

class Program
{
    static int Main(string[] args)
    {
        CreateDefaultAppsettings();

        ParserResult<object> result = Parser.Default.ParseArguments<InstallVerb, UninstallVerb, InitVerb>(args);

        int exitCode = result.MapResult(
            (InstallVerb installVerb) => new InstallHandler().Handle(installVerb),
            (UninstallVerb uninstallVerb) => new UninstallHandler().Handle(uninstallVerb),
            (InitVerb initVerb) => new InitHandler().Handle(initVerb),
            (IEnumerable<Error> errors) =>
            {
                foreach (var error in errors)
                {
                    Console.Error.WriteLine(error.ToString());
                }
                return 1;
            }
        );

        return exitCode;
    }

    static int CreateDefaultAppsettings()
    {
        if (!Directory.Exists(Config.CPMDir))
            Directory.CreateDirectory(Config.CPMDir);

        if (!File.Exists(Path.Combine(Config.CPMDir, "cpmsettings.json")))
            File.WriteAllText(Path.Combine(Config.CPMDir, "cpmsettings.json"), """
            {
              "RepositoryUrl": "http://localhost:5001/api",
              "CachePath": "cache"
            }
            """);
        return 0;
    }
}