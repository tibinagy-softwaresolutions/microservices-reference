namespace TNArch.Microservices.Core.Common.DependencyInjection
{
    public enum DependencyLifeStyle : ushort
    {
        Singleton = 0,
        Scoped = 1,
        Transient = 2,
    }
}