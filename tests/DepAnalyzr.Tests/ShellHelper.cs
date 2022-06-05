using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DepAnalyzr.Tests;

public static class ShellHelper
{
    public static async Task<T> ExecuteWithinDirectory<T>(
        string directory, Func<CancellationToken,Task<T>> act, CancellationToken ct)
    {
        var prevDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = directory;

        try
        {
            return await act(ct);
        }
        finally
        {
            Environment.CurrentDirectory = prevDirectory;
        }
    }

    public static async Task<int> ExecuteProcess(string fileName, string arguments, CancellationToken ct)
    {
        var processStartInfo = new ProcessStartInfo(fileName, arguments) { RedirectStandardOutput = true };
        using var process = Process.Start(processStartInfo);
        await process!.WaitForExitAsync(ct);
        return process.ExitCode;
    }
}