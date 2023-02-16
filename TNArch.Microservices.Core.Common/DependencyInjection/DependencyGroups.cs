namespace TNArch.Microservices.Core.Common.DependencyInjection
{
    [Flags]
    public enum DependencyGroups : ushort
    {
        Function = 1,
        Web = 2,
        All = Function | Web,
    }
}