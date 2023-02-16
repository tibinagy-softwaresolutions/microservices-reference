namespace TNArch.Microservices.Core.Common.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationDescriptorAttribute : Attribute
    {
        public ConfigurationDescriptorAttribute(string configurationPath = null, bool isCollection = false)
        {
            ConfigurationPath = configurationPath;
            IsCollection = isCollection;
        }

        public string ConfigurationPath { get; }
        public bool IsCollection { get; }
    }
}
