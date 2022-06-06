using System.Diagnostics;

namespace DepAnalyzr.Utilities;

public static class ShellExecutor
{
    public static async Task<T> ExecuteWithinDirectory<T>(
        string directory, Func<CancellationToken, Task<T>> act, CancellationToken ct)
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

    public static int ExecuteProcess(string fileName, string arguments)
    {
        var processStartInfo = new ProcessStartInfo(fileName, arguments) { RedirectStandardOutput = true };
        using var process = Process.Start(processStartInfo);
        process!.WaitForExit();
        return process.ExitCode;
    }

    public static (int exitCode, string stdOutput, string stdError)
        ExecuteProcessReadingStreams(string fileName, string arguments)
    {
        var processStartInfo = new ProcessStartInfo(fileName, arguments)
            { RedirectStandardOutput = true, RedirectStandardError = true };

        using var process = Process.Start(processStartInfo);
        process!.WaitForExit();
        
        var standardOutput = process.StandardOutput.ReadToEnd();
        var standardError = process.StandardError.ReadToEnd();

        return (process.ExitCode, standardOutput, standardError);
    }
}