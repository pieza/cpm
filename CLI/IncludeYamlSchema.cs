namespace cpm.CLI
{
    class IncludeYamlSchema
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public Dictionary<string, string>? Dependencies { get; set; }
        public Dictionary<string, string>? Scripts { get; set; }
    }
}
