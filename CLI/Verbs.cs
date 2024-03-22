using CommandLine;

namespace cpm.CLI
{
    [Verb("install", false, ["i"], HelpText = "Install a package")]
    class InstallVerb
    {
        [Value(1, MetaName = "package", Required = false, HelpText = "Name of the package to install.")]
        public string? PackageIdentifier { get; set; }

        [Value(2, MetaName = "version", Required = false, HelpText = "Version of the package to install.")]
        public string? PackageVersion { get; set; } = "latest";

        [Option('g', "global", HelpText = "Install the package globally.")]
        public bool IsGlobal { get; set; }

        [Option('f', "force", HelpText = "Force to reinstall the package.")]
        public bool IsForced {  get; set; }
    }

    [Verb("uninstall", false, ["u"], HelpText = "Uninstall a package")]
    class UninstallVerb
    {
        [Value(1, MetaName = "package", Required = true, HelpText = "Name of the package to uninstall.")]
        public string? PackageName { get; set; }

        [Option('g', "global", HelpText = "Uninstall the package globally.")]
        public bool IsGlobal { get; set; }
    }

    [Verb("init", false, HelpText = "Creates a default includes.yml file.")]
    class InitVerb
    {
        [Value(1, MetaName = "name", Required = false, HelpText = "Name of the project, if provided, will create a subdirectory.")]
        public string? ProjectName { get; set; }
    }

    [Verb("run", true, [""], HelpText = "Runs a script from the include.yaml scripts section.")]
    class RunVerb
    {
        [Value(1, MetaName = "script", Required = true, HelpText = "Script to run.")]
        public required string Script { get; set; }

    }
}
