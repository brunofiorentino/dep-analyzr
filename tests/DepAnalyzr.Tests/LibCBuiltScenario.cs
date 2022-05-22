using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static DepAnalyzr.Tests.Shared.ShellHelper;

namespace DepAnalyzr.Tests;

// ReSharper disable once ClassNeverInstantiated.Global
public class LibCBuiltScenario : IAsyncLifetime
{
    public string TargetLibrariesBuildOutputPath { get; private set; }
    public IReadOnlyList<string> ModulePaths { get; private set; }
    public string ModuleAPath { get; private set; }
    public string ModuleBPath { get; private set; }
    public string ModuleCPath { get; private set; }

    public async Task InitializeAsync()
    {
        var dotNetCliPath = Environment.OSVersion.Platform switch
        {
            PlatformID.Win32NT => @"C:\Program Files\dotnet\dotnet.exe", // TODO: Drive letter shouldn't be hard coded.
            PlatformID.Unix => "/usr/bin/dotnet", // TODO: Consider dynamic way to obtain this path.
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

        ModulePaths = new[] {'A', 'B', 'C'}
            .Select(x => $"DepAnalyzr.Tests.Lib{x}.dll")
            .Select(x => Path.Combine(TargetLibrariesBuildOutputPath, x))
            .ToList();

        Assert.All(ModulePaths, path => Assert.True(File.Exists(path)));

        var index = -1;
        ModuleAPath = ModulePaths[++index];
        ModuleBPath = ModulePaths[++index];
        ModuleCPath = ModulePaths[++index];
    }

    public Task DisposeAsync()
    {
        if (Directory.Exists(TargetLibrariesBuildOutputPath))
            Directory.Delete(TargetLibrariesBuildOutputPath, true);

        return Task.CompletedTask;
    }
}