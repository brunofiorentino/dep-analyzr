using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DepAnalyzr.Tests.Shared;

public static class ShellHelper
{
    public static async Task<T> ExecuteWithinDirectory<T>(string directory, Func<Task<T>> act)
    {
        var prevDirectory = Environment.CurrentDirectory;
        Environment.CurrentDirectory = directory;
        
        try
        {
            var value = await act();
            return value;
        }
        finally
        {
            Environment.CurrentDirectory = prevDirectory;    
        }
    }

    public static async Task<int> ExecuteProcess(string fileName, string arguments)
    {
        var processStartInfo = new ProcessStartInfo(fileName, arguments)
        {
            RedirectStandardOutput = true
        };
        
        using var process = Process.Start(processStartInfo);
        await process!.WaitForExitAsync();
        return process.ExitCode;
    }
}