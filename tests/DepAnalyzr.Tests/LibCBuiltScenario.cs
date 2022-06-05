using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static DepAnalyzr.Tests.ShellHelper;

namespace DepAnalyzr.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class LibCBuiltScenario : IAsyncLifetime
{
    private readonly bool _ownCts;
    private readonly CancellationTokenSource? _cts;

    public LibCBuiltScenario(CancellationTokenSource? cts)
    {
        _ownCts = cts is null;
        _cts = _ownCts ? new CancellationTokenSource() : cts;
    }

    public IEnumerable<string> AssemblyPaths { get; private set; } = null!;

    public async Task InitializeAsync() =>
        await GivenLibCAndDependenciesWereBuilt();

    private async Task GivenLibCAndDependenciesWereBuilt()
    {
        var dotNetCliPath = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => Path.Combine(
                Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System))!,
                "Program Files\\dotnet\\dotnet.exe"),

            PlatformID.Unix => "/usr/bin/dotnet",

            _ => throw new NotImplementedException()
        };

        var libCDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../", "DepAnalyzr.Tests.LibC");
        var targetLibrariesBuildOutputPath = Environment.CurrentDirectory;

        var dotnetBuildArguments = $"build -o \"{targetLibrariesBuildOutputPath}\"";
        var dotnetBuildLibCExitCode = await ExecuteWithinDirectory(
            libCDirectory, async ct => await ExecuteProcess(dotNetCliPath, dotnetBuildArguments, ct), _cts!.Token);

        Assert.Equal(0, dotnetBuildLibCExitCode);

        AssemblyPaths = new[] { 'A', 'B', 'C' }
            .Select(x => $"DepAnalyzr.Tests.Lib{x}.dll")
            .Select(x => Path.Combine(targetLibrariesBuildOutputPath, x))
            .ToList();

        Assert.All(AssemblyPaths, path => Assert.True(File.Exists(path)));
    }

    public Task DisposeAsync()
    {
        if (!_ownCts) return Task.CompletedTask;
        _cts!.Cancel();
        _cts.Dispose();
        return Task.CompletedTask;
    }
}