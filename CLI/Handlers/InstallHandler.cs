using cpm.Services;
using cpm.Utils;
using System.IO;

namespace cpm.CLI.Handlers
{
    class InstallHandler : IVerbHandler<InstallVerb>
    {
        public int Handle(InstallVerb opts)
        {
            string version = !string.IsNullOrEmpty(opts.PackageVersion) ? opts.PackageVersion : "latest";
            Logger.Instance.Info($"Installing package {opts.PackageIdentifier} on version {version}...");
            string currentDirectory = Directory.GetCurrentDirectory();

            if (!File.Exists(Path.Combine(currentDirectory, "include.yaml")))
            {
                Logger.Instance.Error("include.yaml file not found.");
                return 1;
            }

            string includeYamlText = File.ReadAllText(Path.Combine(currentDirectory, "include.yaml"));
            var includeYaml = YamlUtils.Deserialize(includeYamlText) ?? throw new Exception("Could not parse include.yaml");
            Dictionary<string, string> dependencies = includeYaml?.Dependencies != null ? includeYaml.Dependencies : [];
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
                    includeYaml.Dependencies = dependencies;
                    var text = YamlUtils.Serialize(includeYaml);
                    File.WriteAllText(Path.Combine(currentDirectory, "include.yaml"), text);
                }
            }
            else
            {
                foreach (var item in dependencies)
                {
                    InstallPackage(item.Key, item.Value).Wait();
                }
            }


            Logger.Instance.Info($"Package '{opts.PackageIdentifier}' installed successfully!");
            return 0;
        }

        private async Task<string?> InstallPackage(string identifier, string version)
        {
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

            Logger.Instance.Info($"Installing package {identifier} {version} ...");
            string? cachePath = Cache.GetPackagePath(identifier, version);

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

            string[] files = Directory.GetFileSystemEntries(cachePath, "*", SearchOption.AllDirectories);

            foreach (string path in files)
            {
                if (path.Contains(".git"))
                {
                    continue;
                }

                string relativePath = path[(cachePath.Length + 1)..];
                string destinationPath = Path.Combine(targetPath, relativePath);

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                File.Copy(path, destinationPath, true); // Overwrite if file already exists in the destination
                Logger.Instance.Info($"Copied {path} to {destinationPath}");
            }
            Logger.Instance.Info($"Package {identifier} installed successfully!");
            return cachePath.Split(Path.DirectorySeparatorChar).LastOrDefault();
        }
    }
}
