using cpm.Utils;

namespace cpm.CLI.Handlers
{
    class InitHandler : IVerbHandler<InitVerb>
    {
        public int Handle(InitVerb opts)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string projectName = new DirectoryInfo(currentDirectory).Name;
            ;

            if (!string.IsNullOrEmpty(opts.ProjectName))
            {
                projectName = opts.ProjectName;
                string newDirectoryPath = Path.Combine(currentDirectory, projectName);

                if (!Directory.Exists(newDirectoryPath))
                {
                    Directory.CreateDirectory(newDirectoryPath);
                    currentDirectory = newDirectoryPath;
                }
                else
                {
                    Logger.Instance.Error($"Directory '{projectName}' already exists at '{currentDirectory}'");
                    return 1;
                }
            }

            if (Directory.GetFileSystemEntries(currentDirectory).Length == 0)
            {
                CreateHelloWorld(currentDirectory);
            }

            CreateIncludeYamlFile(projectName, currentDirectory);
            CreateMakefile(projectName, currentDirectory);
            CreateGitignore(currentDirectory);

            Logger.Instance.Info($"Project {projectName} initiated succesfully!");
            return 0;
        }

        private void CreateIncludeYamlFile(string projectName, string directory)
        {
            var includesYaml = new IncludeYamlSchema()
            {
                Name = projectName,
                Version = "1.0.0",
                Scripts = new Dictionary<string, string>
                {
                    { "start", "make run" },
                    { "build", "make" }
                }
            };

            var content = YamlUtils.Serialize(includesYaml);
            File.WriteAllText(Path.Combine(directory, "include.yaml"), content);
        }

        private void CreateMakefile(string projectName, string directory)
        {
            var content = DefaultFiles.BuildMakefile(projectName);
            File.WriteAllText(Path.Combine(directory, "Makefile"), content);
        }

        private void CreateHelloWorld(string directory)
        {
            File.WriteAllText(Path.Combine(directory, "main.c"), DefaultFiles.HelloWorld);
        }

        private void CreateGitignore(string directory)
        {
            File.WriteAllText(Path.Combine(directory, ".gitignore"), DefaultFiles.Gitignore);
        }
    }
}
