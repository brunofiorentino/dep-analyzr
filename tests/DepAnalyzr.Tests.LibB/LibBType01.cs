using DepAnalyzr.Tests.LibA;

namespace DepAnalyzr.Tests.LibB;

public class LibBType01
{
    public void DoSomething()
    {
        var libAType01 = new LibAType01();
        libAType01.DoSomething();
    }

    public static double StaticDoSomething()
    {
        return LibAType01.StaticDoSomething();
    }
}