using cpm.Utils;

namespace cpm.CLI.Handlers
{
    class InstallHandler : IVerbHandler<InstallVerb>
    {
        public int Handle(InstallVerb opts)
        {
            string version = !string.IsNullOrEmpty(opts.PackageVersion) ? opts.PackageVersion : "latest";
            var includeYaml = new IncludeYaml();

            if (!includeYaml.Exists)
            {
                Logger.Instance.Error("include.yaml file not found.");
                return 1;
            }

            Dictionary<string, string> dependencies = includeYaml?.Content.Dependencies != null ? includeYaml.Content.Dependencies : [];
            if (!string.IsNullOrEmpty(opts.PackageIdentifier))
            {
                if (dependencies.ContainsKey(opts.PackageIdentifier) && !opts.IsForced)
                {
                    Logger.Instance.Error($"Package '{opts.PackageIdentifier}' is already installed, run with --force to reinstall.");
                    return 1;
                }

                var installedVersion = InstallPackage(opts.PackageIdentifier, version).Result;

                if (includeYaml != null && !string.IsNullOrEmpty(installedVersion))
                {
                    dependencies.Add(opts.PackageIdentifier, installedVersion);
                    includeYaml.Content.Dependencies = dependencies;
                    includeYaml.Save();
                }
            }
            else
            {
                foreach (var item in dependencies)
                {
                    InstallPackage(item.Key, item.Value).Wait();
                }
                Logger.Instance.Info($"All {dependencies.Count} dependencies installed succesfully!");
            }

            return 0;
        }

        private async Task<string?> InstallPackage(string identifier, string version)
        {

            Logger.Instance.Info($"Installing package {identifier} on version {version}...");
            string currentDirectory = Directory.GetCurrentDirectory();
            string includePath = Path.Combine(currentDirectory, ".include");

            if (!Directory.Exists(includePath))
                Directory.CreateDirectory(includePath);

            string targetPath = Path.Combine(includePath, identifier);

            if (Directory.Exists(targetPath))
            {
                Logger.Instance.Info($"Package {identifier} already installed.");
                return null;
            }

            string? cachePath = null;

            if (version != "latest")
            {
                Logger.Instance.Info($"Cheking for package {identifier} {version} on cache ...");
                cachePath = Cache.GetPackagePath(identifier, version);
            }

            if (string.IsNullOrEmpty(cachePath))
            {
                Logger.Instance.Info($"{identifier} is not in cache, proceding to install.");
                var downloadedPackage = await Cache.DownloadPackage(identifier, version);

                if (downloadedPackage == null || string.IsNullOrEmpty(downloadedPackage.Path))
                {
                    Logger.Instance.Error($"Could not install package {identifier} {version}");
                    return null;
                }

                cachePath = downloadedPackage.Path;
            }

            Cache.Copy(cachePath, targetPath);

            Logger.Instance.Info($"Package {identifier} installed successfully!");
            return cachePath.Split(Path.DirectorySeparatorChar).LastOrDefault();
        }
    }
}
