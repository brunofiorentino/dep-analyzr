using DepAnalyzr.Core;

namespace DepAnalyzr.Tests.Core;

public static class HandyNames
{
    public const string LibAAssemblyName = "DepAnalyzr.Tests.LibA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
    public const string LibAType01Name = "DepAnalyzr.Tests.LibA.LibAType01";

    public const string LibBAssemblyName = "DepAnalyzr.Tests.LibB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
    public const string LibBType01Name = "DepAnalyzr.Tests.LibB.LibBType01";

    public const string LibCAssemblyName = "DepAnalyzr.Tests.LibC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
    public const string LibCType01Name = "DepAnalyzr.Tests.LibC.LibCType01";

    // ReSharper disable once InconsistentNaming
    public const string Yes = DependencyMatrix.Yes;

    // ReSharper disable once InconsistentNaming
    public const string No = DependencyMatrix.No;
}