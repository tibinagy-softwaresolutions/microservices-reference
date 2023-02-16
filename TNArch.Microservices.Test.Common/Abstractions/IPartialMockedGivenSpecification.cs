using Moq;

namespace TNArch.Microservices.Test.Common.Abstractions
{
    public interface IPartialMockedGivenSpecification<T> where T : class
    {
        IPartialMockedGivenSpecification<T> GivenPartialMock(Action<Mock<T>> given);
        IPartialMockedGivenSpecification<T> GivenPartialMock<TMock>(Action<Mock<T>, Mock<TMock>> given) where TMock : class;
        IThenSpecification<T> WhenExecute(Action<T> action);
        IThenSpecificationWithResult<T, TResult> WhenExecute<TResult>(Func<T, TResult> action);
    }
}
