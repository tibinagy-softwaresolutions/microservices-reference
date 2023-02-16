using System.Diagnostics;

namespace TNArch.Microservices.Infrastructure.Common.DependencyInjection
{
    public static class LinqExtensions
    {
        [DebuggerStepThrough]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }

}
