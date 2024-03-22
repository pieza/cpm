using cpm.Shell;
using cpm.Utils;

namespace cpm.CLI.Handlers
{
    class RunHandler : IVerbHandler<RunVerb>
    {
        public int Handle(RunVerb opts)
        {
            IncludeYaml includeYaml = new();

            if (!includeYaml.Exists)
            {
                Logger.Instance.Error("include.yaml file not found.");
                return 1;
            }

            var scripts = includeYaml.Content.Scripts ?? [];

            if (!scripts.TryGetValue(opts.Script, out string? command))
                throw new Exception("Script not found.");

            new Command { Script = command }.Run();
            
            return 0;
        }

    }
}
