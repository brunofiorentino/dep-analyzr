namespace DepAnalyzr.Tests.LibA;

public class LibAType01
{
    public int SomeProp { get; set; }
    public static int StaticSomeProp { get; set; }
    
    public void DoSomething()
    {
        
    }
    
    public static double StaticDoSomething()
    {
        var r = new Random(123);
        return r.NextDouble();
    }
}