using cpm.Utils;

namespace cpm.CLI.Handlers
{
    class UninstallHandler : IVerbHandler<UninstallVerb>
    {
        public int Handle(UninstallVerb opts)
        {
            Logger.Instance.Info($"Uninstalling package {opts.PackageName}...");

            Logger.Instance.Info($"Package '{opts.PackageName}' uninstalled successfully!");
            return 0;
        }
    }
}
