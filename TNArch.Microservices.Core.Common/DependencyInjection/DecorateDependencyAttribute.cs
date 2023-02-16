namespace TNArch.Microservices.Core.Common.DependencyInjection
{
    public class DecorateDependencyAttribute : DependencyAttribute
    {
        public DecorateDependencyAttribute(Type serviceType, Type decoratedService, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Scoped, string scope = null) : base(serviceType, decoratedService, true, scope, lifeStyle)
        {
        }
    }
}