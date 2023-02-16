using TNArch.Microservices.Test.Common.Abstractions;

namespace TNArch.Microservices.Test.Common
{
    public static class Specification
    {
        public static IGivenSpecification<T> ForService<T>() where T : class
        {
            return new ServiceSpecification<T>();
        }
    }
}
