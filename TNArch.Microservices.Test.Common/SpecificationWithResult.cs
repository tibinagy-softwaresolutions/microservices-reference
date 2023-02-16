using Microsoft.Extensions.DependencyInjection;
using Moq;
using TNArch.Microservices.Test.Common.Abstractions;

namespace TNArch.Microservices.Test.Common.Testing.Specification
{
    public class SpecificationWithResult<T, TResult> : ServiceSpecification<T>, IThenSpecificationWithResult<T, TResult> where T : class
    {
        private readonly TResult _result;

        public SpecificationWithResult(TResult result, T service, IServiceCollection services, IServiceProvider serviceProvider, Exception occuredException) : base()
        {
            _services = services;
            _result = result;
            _occuredException = occuredException;
            _service = service;
            _serviceProvider = serviceProvider;
        }

        public IThenSpecificationWithResult<T, TResult> ThenResult(Action<TResult> then)
        {
            then(_result);
            return this;
        }

        public IThenSpecificationWithResult<T, TResult> ThenResult<TRes>(Action<TRes> then) where TRes : TResult
        {
            then((TRes)_result!);
            return this;
        }

        IThenSpecificationWithResult<T, TResult> IThenSpecificationWithResult<T, TResult>.ThenExpectedException<TException>(string message)
        {
            ThenExpectedException<TException>(message);
            return this;
        }

        IThenSpecificationWithResult<T, TResult> IThenSpecificationWithResult<T, TResult>.ThenMock<TMock>(Action<Moq.Mock<TMock>> then)
        {
            ThenMock(then);
            return this;
        }

        IThenSpecificationWithResult<T, TResult> IThenSpecificationWithResult<T, TResult>.ThenPartialMock(Action<Mock<T>> then)
        {
            ThenMock(then);
            return this;
        }

        public IThenSpecificationWithResult<T, TResult> WhenExecute(Func<T, TResult> action)
        {
            WhenExecute<TResult>(action);
            return this;
        }

        IThenSpecificationWithResult<T, TResult> IThenSpecificationWithResult<T, TResult>.ThenInfoLog(string informationMessage)
        {
            ThenInfoLog(informationMessage);
            return this;
        }

        IThenSpecificationWithResult<T, TResult> IThenSpecificationWithResult<T, TResult>.ThenExceptionLog<TException>(string exceptionMessage, TException exception)
        {
            ThenExceptionLog(exceptionMessage, exception);
            return this;
        }
    }
}