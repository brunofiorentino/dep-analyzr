using DepAnalyzr.Tests.LibA;
using DepAnalyzr.Tests.LibB;

namespace DepAnalyzr.Tests.LibC;

public class LibCType01
{
    public void DoSomething()
    {
        var libAType01 = new LibAType01();
        libAType01.DoSomething();
        
        var libBType01 = new LibBType01();
        libBType01.DoSomething();
    }
}