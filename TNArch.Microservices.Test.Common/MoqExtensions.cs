using Moq;
using Moq.Language;
using Moq.Language.Flow;
using System.Diagnostics;
using System.Linq.Expressions;
using MockQueryable.Moq;

namespace TNArch.Microservices.Test.Common
{
    public static class MockExtensions
    {
        [DebuggerStepThrough]
        public static void VerifyCalledOnce<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Once);
        }

        [DebuggerStepThrough]
        public static void VerifyCalledOnce<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.Once);
        }

        [DebuggerStepThrough]
        public static void VerifyNeverCalled<T>(this Mock<T> mock, Expression<Action<T>> expression) where T : class
        {
            mock.Verify(expression, Times.Never);
        }

        [DebuggerStepThrough]
        public static void VerifyNeverCalled<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            mock.Verify(expression, Times.Never);
        }

        [DebuggerStepThrough]
        public static IReturnsResult<TMock> ReturnsAsync<TMock>(this IReturns<TMock, Task> mock) where TMock : class
        {
            return mock.Returns(Task.CompletedTask);
        }

        [DebuggerStepThrough]
        public static IReturnsResult<TMock> ReturnsAsyncQueryable<TMock, TElement>(this IReturns<TMock, IQueryable<TElement>> mock, IEnumerable<TElement> value)
             where TMock : class
             where TElement : class
        {
            return mock.Returns(value.AsQueryable().BuildMock());
        }
    }
}
