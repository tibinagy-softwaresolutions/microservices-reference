namespace TNArch.Microservices.Core.Common.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class DependencyAttribute : Attribute
    {
        protected DependencyAttribute(Type serviceType, Type serviceToReplace, bool replaceExistingService, string scope, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Scoped, DependencyGroups dependencyGroup = DependencyGroups.All)
        {
            ServiceType = serviceType;
            ServiceToReplace = serviceToReplace;
            ReplaceExistingService = replaceExistingService;
            Scope = scope;
            LifeStyle = lifeStyle;
            DependencyGroup = dependencyGroup;
        }

        public DependencyAttribute(Type serviceType, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Scoped, string scope = null, DependencyGroups dependencyGroup = DependencyGroups.All)
        {
            ServiceType = serviceType;
            LifeStyle = lifeStyle;
            Scope = scope;
            DependencyGroup = dependencyGroup;
            ReplaceExistingService = false;
        }

        public Type ServiceType { get; }
        public Type ServiceToReplace { get; }
        public DependencyLifeStyle LifeStyle { get; }
        public DependencyGroups DependencyGroup { get; }
        public string Scope { get; }
        public bool ReplaceExistingService { get; }
    }
}