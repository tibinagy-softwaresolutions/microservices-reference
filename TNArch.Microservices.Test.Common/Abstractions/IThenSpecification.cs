using Moq;

namespace TNArch.Microservices.Test.Common.Abstractions
{
    public interface IThenSpecification<T> where T : class
    {
        IThenSpecification<T> ThenMock(Action<Mock<T>> then);
        IThenSpecification<T> ThenMock<TMock>(Action<Mock<TMock>> then) where TMock : class;
        IThenSpecification<T> ThenExpectedException<TException>(string message = null) where TException : Exception;
        IThenSpecification<T> ThenInfoLog(string informationMessage);
        IThenSpecification<T> ThenExceptionLog<TException>(string exceptionMessage, TException exception) where TException : Exception;
        IThenSpecification<T> ThenExceptionLog<TException>(Func<string, bool> exceptionMessageValidator, TException exception) where TException : Exception;
    }
}
