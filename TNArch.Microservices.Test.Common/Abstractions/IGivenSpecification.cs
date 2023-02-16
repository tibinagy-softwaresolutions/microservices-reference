using Moq;

namespace TNArch.Microservices.Test.Common.Abstractions
{
    public interface IGivenSpecification<T> where T : class
    {
        IGivenSpecification<T> GivenMock<TMock>(Action<Mock<TMock>> given) where TMock : class;
        IGivenSpecification<T> GivenMock<TMock1, TMock2>(Action<Mock<TMock1>, Mock<TMock2>> given) where TMock1 : class where TMock2 : class;
        IGivenSpecification<T> GivenDependency<TContract, TService>() where TContract : class where TService : class, TContract;
        IGivenSpecification<T> GivenDependency<TService>(TService instance);
        IGivenSpecification<T> GivenExpectedException<TException>() where TException : Exception;
        IGivenSpecification<T> GivenLogging();
        IPartialMockedGivenSpecification<T> GivenPartialMock(Action<Mock<T>> given);
        IPartialMockedGivenSpecification<T> GivenPartialMock<TMock>(Action<Mock<T>, Mock<TMock>> given) where TMock : class;
        IThenSpecification<T> WhenExecute(Action<T> action);
        IThenSpecificationWithResult<T, TResult> WhenExecute<TResult>(Func<T, TResult> action);
        IThenSpecification<T> WhenInitialized();
    }
}
