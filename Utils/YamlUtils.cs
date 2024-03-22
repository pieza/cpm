using cpm.CLI;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace cpm.Utils
{
    static class YamlUtils
    {
        private static readonly ISerializer _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        private static readonly IDeserializer _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        public static string Serialize(IncludeYamlSchema schema)
        {
            return _serializer.Serialize(schema);
        }

        public static IncludeYamlSchema Deserialize(string yaml)
        {
            return _deserializer.Deserialize<IncludeYamlSchema>(yaml);
        }
    }
}
