using DepAnalyzr.Tests.LibA;
using DepAnalyzr.Tests.LibB;

namespace DepAnalyzr.Tests.LibC;

public class LibCType01
{
    public void DoSomething()
    {
        LibAType01.StaticDoSomething();
        var value1 = LibAType01.StaticSomeProp;
        var libAType01 = new LibAType01();
        libAType01.DoSomething();
        var value2 = libAType01.SomeProp;
        Console.WriteLine(value1 + value2);

        var libBType01 = new LibBType01();
        libBType01.DoSomething();
    }
}