﻿using Agoda.IoC.Generator.UnitTests.Helpers;
using System.Collections;

namespace Agoda.IoC.Generator.UnitTests
{
    public class ContainerRegistrationGeneratorMixLifetimeTests
    {
        [Theory, ClassData(typeof(MixTestDatas))]
        public void Should_Generate_AddScoped_Correctly(string source, string generatedBodyMethod)
        {
            TestHelper.GenerateAgodaIoC(source)
                    .Should()
                    .HaveMethodCount(2)
                    .HaveMethods("Register", "RegisterFromAgodaIoCGeneratorUnitTests")
                    .HaveMethodBody("Register", generatedBodyMethod);
        }
    }


    internal class MixTestDatas : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>
    {
            // case 1
        new object[] { @"
using using Agoda.IoC.Generator.Abstractions;
namespace Agoda.IoC.Generator.UnitTests;

[RegisterScoped]
public class ClassA{
}
[RegisterScoped(ReplaceService = true)]
public class ClassB{
}


", @"serviceCollection.AddScoped<ClassA>();
serviceCollection.Replace(new ServiceDescriptor(typeof(ClassB), ServiceLifetime.Scoped));
return serviceCollection;",

        },
        // case 2
          new object[] { @"
using using Agoda.IoC.Generator.Abstractions;
namespace Agoda.IoC.Generator.UnitTests;
[RegisterScoped(Factory = typeof(ClassBImplementationFactory))]
public class ClassB : IClassB { }
public interface IClassB { }


public class ClassBImplementationFactory : IImplementationFactory<ClassB>
{
    public ClassB Factory(IServiceProvider serviceProvider)
    {
        return new ClassB();
    }
}

[RegisterSingleton(Factory = typeof(ClassCImplementationFactory))]
public class ClassC : IClassC { }
public interface IClassC { }
public class ClassCImplementationFactory : IImplementationFactory<ClassC>
{
    public ClassC Factory(IServiceProvider serviceProvider)
    {
        return new ClassC();
    }
}
[RegisterTransient(ReplaceService =true)]
public class ReplaceA
{
}

public interface IThing<T, U>
{
    string GetNameT { get; }
    string GetNameU { get; }
}
[RegisterScoped(For = typeof(IThing<,>))]
public class GenericThing<T, U> : IThing<T, U>
{
    public GenericThing()
    {
        GetNameT = typeof(T).Name;
        GetNameU = typeof(U).Name;
    }
    public string GetNameT { get; }
    public string GetNameU { get; }
}
", 
@"serviceCollection.AddScoped(sp => new ClassBImplementationFactory().Factory(sp));
serviceCollection.AddSingleton(sp => new ClassCImplementationFactory().Factory(sp));
serviceCollection.Replace(new ServiceDescriptor(typeof(ReplaceA), ServiceLifetime.Transient));
serviceCollection.AddScoped(typeof(IThing<, >), typeof(GenericThing<, >));
return serviceCollection;" },
    };

        public IEnumerator<object[]> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            { return GetEnumerator(); }
        }
    }
}