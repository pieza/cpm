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
                Version = "1.0.0"
            };

            var content = YamlUtils.Serialize(includesYaml);
            File.WriteAllText(Path.Combine(directory, "include.yaml"), content);
        }

        private void CreateMakefile(string projectName, string directory)
        {
            var content = $"""
            CC = gcc
            CFLAGS = -Wall -I./.include
            BUILD_DIR = build

            {projectName}: $(BUILD_DIR)/main.o
            {"\t"}$(CC) $(CFLAGS) -o $(BUILD_DIR)/{projectName} $(BUILD_DIR)/main.o

            $(BUILD_DIR)/main.o: main.c | $(BUILD_DIR)
            {"\t"}$(CC) $(CFLAGS) -c main.c -o $(BUILD_DIR)/main.o

            run: {projectName}
            {"\t"}./$(BUILD_DIR)/{projectName}

            $(BUILD_DIR):
            {"\t"}mkdir -p $(BUILD_DIR)

            clean:
            {"\t"}rm -rf $(BUILD_DIR)
            """;

            File.WriteAllText(Path.Combine(directory, "Makefile"), content);
        }

        private void CreateHelloWorld(string directory)
        {
            var content = """
            #include <stdio.h>

            int main() {
                printf("Hello World!\n");
                return 0;
            }
            """;

            File.WriteAllText(Path.Combine(directory, "main.c"), content);
        }

        private void CreateGitignore(string directory)
        {
            var content = """
            .include/
            build/
            """;

            File.WriteAllText(Path.Combine(directory, ".gitignore"), content);
        }
    }
}
