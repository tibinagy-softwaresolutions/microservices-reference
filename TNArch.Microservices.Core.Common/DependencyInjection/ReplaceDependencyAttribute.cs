namespace TNArch.Microservices.Core.Common.DependencyInjection
{
    public class ReplaceDependencyAttribute : DependencyAttribute
    {
        public ReplaceDependencyAttribute(Type serviceType, Type serviceToReplace = null, string scope = null, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Scoped) : base(serviceType, serviceToReplace, true, scope, lifeStyle)
        {
        }

    }
}