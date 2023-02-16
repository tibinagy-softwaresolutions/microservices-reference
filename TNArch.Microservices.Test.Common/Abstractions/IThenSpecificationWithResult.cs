using Moq;

namespace TNArch.Microservices.Test.Common.Abstractions
{
    public interface IThenSpecificationWithResult<T, TResult> where T : class
    {
        IThenSpecificationWithResult<T, TNewResult> WhenExecute<TNewResult>(Func<T, TNewResult> action);
        IThenSpecificationWithResult<T, TResult> ThenPartialMock(Action<Mock<T>> then);
        IThenSpecificationWithResult<T, TResult> ThenResult(Action<TResult> then);
        IThenSpecificationWithResult<T, TResult> ThenMock<TMock>(Action<Mock<TMock>> then) where TMock : class;
        IThenSpecificationWithResult<T, TResult> ThenExpectedException<TException>(string message = null) where TException : Exception;
        IThenSpecificationWithResult<T, TResult> ThenInfoLog(string informationMessage);
        IThenSpecificationWithResult<T, TResult> ThenExceptionLog<TException>(string exceptionMessage, TException exception) where TException : Exception;
    }
}
