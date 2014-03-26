using System;
using System.Security.Principal;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace LightInject.AutoMoq.Tests
{
    [TestFixture]
    public class MockingServiceContainerTests
    {
        private MockingServiceContainer _container;

        [SetUp]
        public void SetUp()
        {
            _container = new MockingServiceContainer();
        }

        [Test]
        public void MockingContainerShouldResolveNonRegisteredInterfaceAsMock()
        {
            var resolved = _container.GetInstance<INotRegistered>();
            resolved.Should().NotBeNull();
        }

        [Test]
        public void MockingContainerShouldResolveSameInstanceForMultipleResolutions()
        {
            var first = _container.GetInstance<INotRegistered>();
            var second = _container.GetInstance<INotRegistered>();
            ReferenceEquals(first, second).Should().BeTrue();
        }

        [Test]
        public void MockingContainerShouldResolveNonRegisteredClassWithPropertiesAsMock()
        {
            var resolved = _container.GetInstance<NotRegisteredWithProperty>();
            resolved.Should().NotBeNull();
            resolved.Child.Should().NotBeNull();
        }

        [Test]
        public void MockingContainerShouldResolveNonRegisteredClassWithConstructorParametersAsMock()
        {
            var resolved = _container.GetInstance<NotRegisterdWithConstructor>();
            resolved.Should().NotBeNull();
            resolved.Child.Should().NotBeNull();
        }

        [Test]
        public void GetMockShouldReturnTheSameInstanceEachTime()
        {
            var first = _container.GetMock<INotRegistered>();
            var second = _container.GetMock<INotRegistered>();
            first.Should().Be(second);
        }

        [Test]
        public void GetMockShouldAllowSetupForChildren()
        {
            const string expected = "Aaron";
            _container.GetMock<INotRegistered>()
                      .Setup(x => x.Name)
                      .Returns(expected);

            var instance = _container.GetInstance<NotRegisterdWithConstructor>();
            instance.Child.Name.Should().Be(expected);
        }
    }

    public class NotRegisterdWithConstructor
    {
        private readonly INotRegistered _child;

        public NotRegisterdWithConstructor(INotRegistered child)
        {
            _child = child;
        }

        public INotRegistered Child
        {
            get { return _child; }
        }
    }

    public class NotRegisteredWithProperty
    {
        public INotRegistered Child { get; set; }
        public string Name { get; set; }
    }

    public interface INotRegistered
    {
        string Name { get; set; }
    }
}