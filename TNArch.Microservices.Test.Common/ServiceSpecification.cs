using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using TNArch.Microservices.Test.Common.Testing.Specification;
using TNArch.Microservices.Test.Common.Abstractions;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using TNArch.Microservices.Core.Common.Extensions;

namespace TNArch.Microservices.Test.Common
{
    public class ServiceSpecification<T> : IGivenSpecification<T>, IPartialMockedGivenSpecification<T>, IThenSpecification<T> where T : class
    {
        protected IServiceCollection _services;
        protected IServiceProvider _serviceProvider;
        protected T _service;
        protected Type _expectedExceptionType;
        protected Exception _occuredException;

        public ServiceSpecification()
        {
            _services = new ServiceCollection();
            _services.AddScoped(typeof(T));
            _serviceProvider = _services.BuildServiceProvider();
        }

        public IGivenSpecification<T> GivenDependency<TContract, TService>()
            where TContract : class
            where TService : class, TContract
        {
            var existingService = _services.FirstOrDefault(s => s.ServiceType == typeof(TContract));

            if (existingService != null)
                _services.Remove(existingService);

            RegisterService<TContract, TService>();

            return this;
        }

        public IGivenSpecification<T> GivenDependency<TService>(TService instance)
        {
            var existingService = _services.FirstOrDefault(s => s.ServiceType == typeof(TService));

            if (existingService != null)
                _services.Remove(existingService);

            _services.Add(new ServiceDescriptor(typeof(TService), instance!));

            return this;
        }

        public IGivenSpecification<T> GivenMock<TService>(Action<Mock<TService>> given) where TService : class
        {
            given(GetMock<TService>());

            return this;
        }

        public IGivenSpecification<T> GivenMock<TService1, TService2>(Action<Mock<TService1>, Mock<TService2>> given)
            where TService1 : class
            where TService2 : class
        {
            given(GetMock<TService1>(), GetMock<TService2>());

            return this;
        }

        public IGivenSpecification<T> GivenLogging()
        {
            GetMock<ILogger<T>>().Setup(x => x.Log(It.IsAny<LogLevel>(),
                       It.IsAny<EventId>(),
                       It.IsAny<It.IsAnyType>(),
                       It.IsAny<Exception>(),
                       It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            return this;
        }

        public IPartialMockedGivenSpecification<T> GivenPartialMock(Action<Mock<T>> given)
        {
            RegisterService<T, T>();

            given(_serviceProvider.GetRequiredService<Mock<T>>());

            return this;
        }


        public IPartialMockedGivenSpecification<T> GivenPartialMock<TMock>(Action<Mock<T>, Mock<TMock>> given) where TMock : class
        {
            RegisterService<T, T>();

            given(_serviceProvider.GetRequiredService<Mock<T>>(), GetMock<TMock>());

            return this;
        }

        private void RegisterService<TContract, TService>() where TService : TContract
        {
            var existingMock = _serviceProvider.GetService<Mock<T>>();

            if (existingMock != null)
                return;

            var serviceType = typeof(TService);

            var mockType = typeof(Mock<>).MakeGenericType(serviceType);

            var dependencyTypes = serviceType.GetConstructors()
                .First()
                .GetParameters()
                .Select(d => d.ParameterType)
                .Distinct()
                .ToArray();

            dependencyTypes.Except(new[] { typeof(IServiceProvider) })
                .Where(dt => !_services.Any(s => s.ServiceType == dt))
                .ForEach(d => RegisterMock(d));

            _serviceProvider = _services.BuildServiceProvider();

            var serviceMock = (Mock)Activator.CreateInstance(mockType, dependencyTypes.Select(dt => _serviceProvider.GetRequiredService(dt)).ToArray())!;
            serviceMock.CallBase = true;

            _services.Add(new ServiceDescriptor(mockType, serviceMock));
            _services.Replace(new ServiceDescriptor(typeof(TContract), serviceMock.Object));

            _serviceProvider = _services.BuildServiceProvider();
        }

        public IGivenSpecification<T> GivenExpectedException<TException>() where TException : Exception
        {
            _expectedExceptionType = typeof(TException);

            return this;
        }

        public IThenSpecification<T> WhenExecute(Action<T> action)
        {
            try
            {
                RegisterService<T, T>();

                _service ??= _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider.GetRequiredService<T>();

                action(_service);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == _expectedExceptionType)
                    ex = ex.InnerException!;

                if (_expectedExceptionType != ex.GetType())
                    throw;

                _occuredException = ex;
            }

            return this;
        }

        public IThenSpecification<T> WhenInitialized()
        {
            try
            {
                RegisterService<T, T>();

                _service ??= _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider.GetRequiredService<T>();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == _expectedExceptionType)
                    ex = ex.InnerException!;

                if (_expectedExceptionType != ex.GetType())
                    throw;

                _occuredException = ex;
            }

            return this;
        }

        public IThenSpecificationWithResult<T, TResult> WhenExecute<TResult>(Func<T, TResult> action)
        {
            if (typeof(TResult) == typeof(Task) || typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>))
                throw new InvalidOperationException("Action should not contain async Task Result. Don't forget to call Wait() or access Result property in WhenExecute");

            var result = default(TResult);

            try
            {
                RegisterService<T, T>();

                _service ??= _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider.GetRequiredService<T>();

                result = action(_service);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.GetType() == _expectedExceptionType)
                    ex = ex.InnerException!;

                if (_expectedExceptionType != ex.GetType())
                    throw;

                _occuredException = ex;
            }

            return new SpecificationWithResult<T, TResult>(result!, _service!, _services, _serviceProvider, _occuredException!);
        }

        public IThenSpecification<T> ThenMock<TService>(Action<Mock<TService>> then) where TService : class
        {
            then(_serviceProvider.GetRequiredService<Mock<TService>>());

            return this;
        }

        public IThenSpecification<T> ThenMock(Action<Mock<T>> then)
        {
            then(_serviceProvider.GetRequiredService<Mock<T>>());

            return this;
        }

        public IThenSpecification<T> ThenExpectedException<TException>(string message = null) where TException : Exception
        {
            _occuredException.Should().NotBeNull();
            _occuredException.Should().BeOfType<TException>();

            if (message != null)
                _occuredException!.Message.Should().Contain(message);

            return this;
        }

        private void RegisterMock(Type dependencyType)
        {
            var mock = (Mock)Activator.CreateInstance(typeof(Mock<>).MakeGenericType(dependencyType), MockBehavior.Strict)!;
            _services.Add(new ServiceDescriptor(mock.GetType(), mock));
            _services.Add(new ServiceDescriptor(mock.GetType().GetGenericArguments().First(), mock.Object));
        }

        private Mock<TService> GetMock<TService>() where TService : class
        {
            var existingMock = _serviceProvider.GetService<Mock<TService>>();

            if (existingMock != null)
                return existingMock;

            RegisterMock(typeof(TService));

            _serviceProvider = _services.BuildServiceProvider();

            return _serviceProvider.GetRequiredService<Mock<TService>>();
        }

        public IThenSpecification<T> ThenInfoLog(string informationMessage)
        {
            GetMock<ILogger<T>>()
               .Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => o.ToString() == informationMessage), null, It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            return this;
        }

        public IThenSpecification<T> ThenExceptionLog<TException>(string exceptionMessage, TException exception) where TException : Exception
        {
            GetMock<ILogger<T>>()
               .Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => o.ToString() == exceptionMessage), exception, It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            return this;
        }

        public IThenSpecification<T> ThenExceptionLog<TException>(Func<string, bool> exceptionMessageValidator, TException exception) where TException : Exception
        {
            GetMock<ILogger<T>>()
               .VerifyCalledOnce(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((o, t) => exceptionMessageValidator(o.ToString()!)), exception, It.IsAny<Func<It.IsAnyType, Exception, string>>()));

            return this;
        }
    }
}