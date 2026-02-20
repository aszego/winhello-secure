using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace WhSecure.Rdp;

internal static class RdpLaunchHelper
{
    private const string PasswordLinePrefix = "password 51:b:";
    private const string UsernameLinePrefix = "username:s:";
    private const string UseMultimonPrefix = "use multimon:i:";
    private const int DefaultWaitMs = 10_000;

    /// <summary>
    /// Builds the RDP "password 51:b:&lt;base64&gt;" value from plain password using DPAPI (CurrentUser).
    /// Uses Unicode (UTF-16LE) to match PowerShell/Windows.
    /// </summary>
    public static string BuildPassword51Line(string plainPassword)
    {
        var bytes = Encoding.Unicode.GetBytes(plainPassword);
        var protectedBytes = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
        return PasswordLinePrefix + Convert.ToHexString(protectedBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Reads source RDP lines, applies username/password/multimon overrides, writes to temp file.
    /// Returns the path to the temp RDP file.
    /// </summary>
    public static string WriteTempRdp(string sourceRdpPath, string username, string plainPassword, bool useMultiMon)
    {
        var lines = File.ReadAllLines(sourceRdpPath);
        var passwordLine = BuildPassword51Line(plainPassword);
        var usernameLine = UsernameLinePrefix + username;
        var multimonLine = UseMultimonPrefix + (useMultiMon ? "1" : "0");

        var result = new List<string>();
        var seenUsername = false;
        var seenPassword = false;
        var seenMultimon = false;

        foreach (var line in lines)
        {
            if (line.StartsWith(UsernameLinePrefix, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(usernameLine);
                seenUsername = true;
                continue;
            }
            if (line.StartsWith(PasswordLinePrefix, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(passwordLine);
                seenPassword = true;
                continue;
            }
            if (line.StartsWith(UseMultimonPrefix, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(multimonLine);
                seenMultimon = true;
                continue;
            }
            result.Add(line);
        }

        if (!seenUsername)
            result.Add(usernameLine);
        if (!seenPassword)
            result.Add(passwordLine);
        if (!seenMultimon)
            result.Add(multimonLine);

        var tempPath = Path.Combine(Path.GetTempPath(), "winhello-secure-" + Guid.NewGuid().ToString("N") + ".rdp");
        File.WriteAllLines(tempPath, result);
        return tempPath;
    }

    public static void LaunchMstsc(string tempRdpPath, int waitMs = DefaultWaitMs)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "mstsc.exe",
            ArgumentList = { tempRdpPath },
            UseShellExecute = true,
        });
        if (process == null)
            throw new InvalidOperationException("Failed to start mstsc.exe.");
        Thread.Sleep(waitMs);
        try
        {
            if (File.Exists(tempRdpPath))
                File.Delete(tempRdpPath);
        }
        catch
        {
            // Ignore cleanup failure
        }
    }
}
