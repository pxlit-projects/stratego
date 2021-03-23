using Moq;

namespace Stratego.TestTools.Builders
{
    public abstract class MockBuilder<T> where T:class
    {
        public Mock<T> Mock { get; private set; }

        public T Object => Mock.Object;

        protected MockBuilder()
        {
            Mock = new Mock<T>();
        }
    }
}