using cpm.DTOs;
using cpm.Services;
using LibGit2Sharp;

namespace cpm.Utils
{
    static class Cache
    {
        public static string GetCachePath()
        {
            string? cachePath = Config.GetProperty("CachePath");

            if (string.IsNullOrEmpty(Config.CPMDir))
                throw new Exception("No cpm directory configured.");

            if (string.IsNullOrEmpty(cachePath))
                throw new Exception("No cache path configured.");

            string fullPath = Path.Combine(Config.CPMDir, cachePath);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            return fullPath;
        }

        public static string? GetPackagePath(string identifier, string version)
        {
            string cachePath = GetCachePath();
            string fullPath = Path.Combine(cachePath, identifier, version);

            if (Directory.Exists(fullPath)) return fullPath;
            return null;
        }

        public static async Task<CachedPackageDTO?> DownloadPackage(string identifier, string version)
        {
            string cachePath = GetCachePath();
            if (version != "latest")
            {
                string fullPath = Path.Combine(cachePath, identifier, version);
                if (Directory.Exists(fullPath) && Directory.EnumerateFileSystemEntries(fullPath).Any())
                    return new() { Path = fullPath, Identifier = identifier, Version = version };
            }

            string packagePath = Path.Combine(cachePath, identifier);

            if (!Directory.Exists(packagePath))
                Directory.CreateDirectory(packagePath);

            var package = await PackagesService.Find(identifier) ?? throw new Exception($"Package {identifier} not found.");

            try
            {
                Logger.Instance.Info($"Looking for version {version}");
                var tags = await GitUtils.GetRepositoryTagsAsync(package.Owner, package.Repo);
                var targetTag = (version == "latest" ? GitUtils.GetLatestTag(tags) : tags.Where(t => t.Name == $"v{version}").FirstOrDefault()) ?? throw new Exception("Version not found");

                string fullPath = Path.Combine(packagePath, targetTag.Name.Replace("v", ""));
                
                if(Directory.Exists(fullPath)) return new() { Path = fullPath, Identifier = identifier, Version = targetTag.Name.Replace("v", "") };

                GitUtils.CloneRepository(package.Owner, package.Repo, fullPath, targetTag.Name);
                return new() { Path = fullPath, Identifier = identifier, Version = targetTag.Name.Replace("v", "") };
            }
            catch (Exception ex)
            {
                Logger.Instance.Error(ex.Message);
                return null;
            }
        }
    }
}
