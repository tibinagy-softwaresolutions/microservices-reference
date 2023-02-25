using System.Diagnostics;
using System.Reflection;

namespace TNArch.Microservices.Core.Common.Extensions
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

        [DebuggerStepThrough]
        public static async Task<IEnumerable<TResult>> SelectManyAsync<T, TResult>(this List<T> enumeration, Func<T, Task<List<TResult>>> func)
        {
            return (await Task.WhenAll(enumeration.Select(func))).SelectMany(s => s);
        }
    }

    public static class TypeExtensions
    {
        public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);

            await task.ConfigureAwait(false);

            return task.GetType().GetProperty(nameof(Task<Object>.Result)).GetValue(task);
        }

        public static MethodInfo GetGenericMethod(this Type type, string methodName, params Type[] genericArguments)
        {
            return type.GetMethods()
                       .First(m => m.Name == methodName && m.IsGenericMethod && m.GetGenericArguments().Length == genericArguments.Length)
                       .MakeGenericMethod(genericArguments);
        }
    }


}
