using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static DepAnalyzr.Tests.ShellHelper;

namespace DepAnalyzr.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class LibCBuiltScenario : IAsyncLifetime
{
    public string TargetLibrariesBuildOutputPath { get; private set; }

    public async Task InitializeAsync()
    {
        var dotNetCliPath = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => @"C:\Program Files\dotnet\dotnet.exe",
            PlatformID.Unix => throw new NotImplementedException(),
            _ => throw new NotImplementedException()
        };

        var libCDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../", "DepAnalyzr.Tests.LibC");
        var tempPathBase = Path.GetTempPath();
        var tempPathRandomPart = Guid.NewGuid().ToString();
        TargetLibrariesBuildOutputPath = Path.Combine(tempPathBase, nameof(DepAnalyzr), tempPathRandomPart);

        var dotnetBuildArguments = $"build -o \"{TargetLibrariesBuildOutputPath}\"";
        var dotnetBuildLibCExitCode = await ExecuteWithinDirectory(
            libCDirectory, async () => await ExecuteProcess(dotNetCliPath, dotnetBuildArguments));

        Assert.Equal(0, dotnetBuildLibCExitCode);

        var expectedBuiltAssemblyPaths = new[] {'A', 'B', 'C'}
            .Select(x => $"DepAnalyzr.Tests.Lib{x}.dll")
            .Select(x => Path.Combine(TargetLibrariesBuildOutputPath, x));

        Assert.All(expectedBuiltAssemblyPaths, path => Assert.True(File.Exists(path)));
    }

    public Task DisposeAsync()
    {
        if (Directory.Exists(TargetLibrariesBuildOutputPath))
            Directory.Delete(TargetLibrariesBuildOutputPath, true);

        return Task.CompletedTask;
    }
}