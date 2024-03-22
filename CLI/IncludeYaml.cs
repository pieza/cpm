
using cpm.Utils;

namespace cpm.CLI
{
    class IncludeYaml
    {
        private readonly string _currentDirectory;
        private readonly string _filename = "include.yaml";
        public IncludeYamlSchema Content { get; set; }
        public string Path { get; private set; }
        public bool Exists
        {
            get
            {
                return File.Exists(Path);
            }
        }

        public IncludeYaml()
        {
            _currentDirectory = Directory.GetCurrentDirectory();
            Path = System.IO.Path.Combine(_currentDirectory, _filename);
            Content = YamlUtils.Deserialize(GetText()) ?? throw new Exception("Could not parse include.yaml");
        }

        public IncludeYaml(string currentDirectory)
        {
            _currentDirectory = currentDirectory;
            Path = System.IO.Path.Combine(_currentDirectory, _filename);
            Content = YamlUtils.Deserialize(GetText()) ?? throw new Exception("Could not parse include.yaml");
        }

        public string GetText()
        {
            return File.ReadAllText(Path);
        }

        public void Save()
        {
            var text = YamlUtils.Serialize(Content);
            File.WriteAllText(Path, text);
        }
    }
}
