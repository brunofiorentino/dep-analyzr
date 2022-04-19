namespace DepAnalyzr.Tests.LibA;

public class LibAType01
{
    public void DoSomething()
    {
        
    }
    
    public static double StaticDoSomething()
    {
        var r = new Random(123);
        return r.NextDouble();
    }
}