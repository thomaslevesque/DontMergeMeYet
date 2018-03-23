using System;
using FakeItEasy;
using FakeItEasy.Creation;

namespace DontMergeMeYet.Tests
{
    public class TestFixtureBase
    {
        protected static void InitFake<T>(out T field, Action<IFakeOptions<T>> optionsBuilder = null)
        {
            field = A.Fake(optionsBuilder ?? (o => { }));
        }
    }
}
