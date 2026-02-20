// winhello-secure - part of https://github.com/aszego/winhello-secure (GPL-3.0)

namespace WhSecure;
using AuthProviders;
using WhSecure.Rdp;
using System.Text;

internal class Program
{
    private const int ExitUsage = 1;
    private const int ExitFormat = 2;
    private const int ExitUserCancel = 3;
    private const int ExitSystem = 4;
    private const int ExitMissingCredential = 5;

    static int Main(string[] args)
    {
        Console.WriteLine("winhello-secure - license (GNU GPLv3), source, releases: https://github.com/aszego/winhello-secure");
        Console.WriteLine("");
        if (args.Length == 0)
        {
            PrintUsage();
            return ExitUsage;
        }

        // Import mode: /importRdp <path> /username <username>
        if (string.Equals(args[0], "/importRdp", StringComparison.OrdinalIgnoreCase))
        {
            return RunImportMode(args);
        }

        // Legacy encrypt/decrypt mode
        if (args.Length == 2 && (args[0] == "encrypt" || args[0] == "decrypt"))
        {
            return RunEncryptDecryptMode(args);
        }

        // Launch mode: <rdpPath> [/multiMon]
        return RunLaunchMode(args);
    }

    private static void PrintUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine(" RDP:");
        Console.WriteLine("  winhello-secure.exe /importRdp <path> /username <username>");
        Console.WriteLine("    Import RDP credentials: prompts for password, encrypts with Windows Hello, stores in JSON under %LOCALAPPDATA%.");
        Console.WriteLine();
        Console.WriteLine("  winhello-secure.exe <rdpPath> [/multiMon]");
        Console.WriteLine("    Launch RDP: looks up credentials, decrypts, creates temp RDP with password, runs mstsc.");
        Console.WriteLine("                /multiMon patches the RDP file to launch with multi monitor layout.");
        Console.WriteLine();
        Console.WriteLine(" General (eg. in your scripts):");
        Console.WriteLine("  winhello-secure.exe [encrypt|decrypt] <base64data>");
        Console.WriteLine("    Returns base64-encoded encrypted or decrypted data. Uses Windows Hello for cryptography.");
        Console.WriteLine();
    }

    private static int RunImportMode(string[] args)
    {
        string? rdpPath = null;
        string? username = null;
        for (var i = 1; i < args.Length; i++)
        {
            if (string.Equals(args[i], "/username", StringComparison.OrdinalIgnoreCase))
            {
                if (i + 1 < args.Length)
                {
                    username = args[i + 1];
                    i++;
                }
                continue;
            }
            if (!args[i].StartsWith("/"))
            {
                rdpPath ??= args[i];
            }
        }

        if (string.IsNullOrEmpty(rdpPath) || string.IsNullOrEmpty(username))
        {
            Console.Error.WriteLine("Import mode requires: /importRdp <path> /username <username>");
            PrintUsage();
            return ExitUsage;
        }

        Console.Write("Mask password? (y/n) [y]: ");
        var maskLine = Console.ReadLine();
        var mask = string.IsNullOrWhiteSpace(maskLine) || maskLine!.Trim().StartsWith("y", StringComparison.OrdinalIgnoreCase);

        string password;
        if (mask)
        {
            Console.Write("Password: ");
            password = ReadMaskedPassword();
            Console.WriteLine();
        }
        else
        {
            Console.Write("Password: ");
            var pwLeft = Console.CursorLeft;
            var pwTop = Console.CursorTop;
            password = Console.ReadLine() ?? "";
            OverwriteLineWithStars(password.Length, pwLeft, pwTop);
        }

        if (string.IsNullOrEmpty(password))
        {
            Console.Error.WriteLine("Password cannot be empty.");
            return ExitUsage;
        }

        try
        {
            Console.WriteLine("Encrypting credentials...");
            var provider = WinHelloProvider.CreateInstance(AuthCacheType.Local);
            var plainBytes = Encoding.UTF8.GetBytes(password);
            var encrypted = provider.Encrypt(plainBytes);
            var encryptedBase64 = Convert.ToBase64String(encrypted);
            var normalizedPath = RdpCredentialStore.NormalizePath(rdpPath);
            RdpCredentialStore.Upsert(normalizedPath, username, encryptedBase64);
            Console.WriteLine("Credentials stored for: " + normalizedPath);
            return 0;
        }
        catch (AuthProviderUserCancelledException)
        {
            return ExitUserCancel;
        }
        catch (AuthProviderSystemErrorException)
        {
            return ExitSystem;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitSystem;
        }
    }

    private static string ReadMaskedPassword()
    {
        var sb = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
                break;
            if (key.Key == ConsoleKey.Backspace && sb.Length > 0)
            {
                sb.Length--;
                Console.Write("\b \b");
                continue;
            }
            if (!char.IsControl(key.KeyChar))
            {
                sb.Append(key.KeyChar);
                Console.Write('*');
            }
        }
        return sb.ToString();
    }

    private static void OverwriteLineWithStars(int length, int cursorLeft, int cursorTop)
    {
        if (length <= 0)
            return;
        Console.SetCursorPosition(cursorLeft, cursorTop);
        Console.Write(new string('*', Math.Min(length, 80)));
        Console.WriteLine();
    }

    private static int RunLaunchMode(string[] args)
    {
        string? rdpPath = null;
        var useMultiMon = false;
        foreach (var arg in args)
        {
            if (string.Equals(arg, "/multiMon", StringComparison.OrdinalIgnoreCase))
            {
                useMultiMon = true;
                continue;
            }
            if (!arg.StartsWith("/"))
            {
                rdpPath ??= arg;
            }
        }

        if (string.IsNullOrEmpty(rdpPath))
        {
            Console.Error.WriteLine("Launch mode requires an RDP file path.");
            PrintUsage();
            return ExitUsage;
        }

        var normalizedPath = RdpCredentialStore.NormalizePath(rdpPath);
        var entry = RdpCredentialStore.Get(normalizedPath);
        if (entry == null)
        {
            Console.Error.WriteLine("No stored credentials for: " + normalizedPath);
            Console.Error.WriteLine("Run with /importRdp first.");
            return ExitMissingCredential;
        }

        if (!File.Exists(rdpPath))
        {
            Console.Error.WriteLine("RDP file not found: " + rdpPath);
            return ExitUsage;
        }

        try
        {
            Console.WriteLine("Invoking Windows Hello to decrypt stored password...");
            var provider = WinHelloProvider.CreateInstance(AuthCacheType.Local);
            var encryptedBytes = Convert.FromBase64String(entry.EncryptedPasswordBase64);
            var decryptedBytes = provider.PromptToDecrypt(encryptedBytes);
            var plainPassword = Encoding.UTF8.GetString(decryptedBytes);

            var tempRdpPath = RdpLaunchHelper.WriteTempRdp(rdpPath, entry.Username, plainPassword, useMultiMon);
            Console.WriteLine("Launching RDP client...");
            RdpLaunchHelper.LaunchMstsc(tempRdpPath);
            return 0;
        }
        catch (AuthProviderUserCancelledException)
        {
            return ExitUserCancel;
        }
        catch (AuthProviderKeyNotFoundException)
        {
            Console.Error.WriteLine("Windows Hello key not found. Re-import credentials.");
            return ExitMissingCredential;
        }
        catch (AuthProviderSystemErrorException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitSystem;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return ExitSystem;
        }
    }

    private static int RunEncryptDecryptMode(string[] args)
    {
        var base64argument = args[1];
        try
        {
            var argument = Convert.FromBase64String(base64argument).ToArray();
            var provider = WinHelloProvider.CreateInstance(AuthCacheType.Local);

            switch (args[0])
            {
                case "encrypt":
                    var encrypted = provider.Encrypt(argument);
                    Console.WriteLine(Convert.ToBase64String(encrypted));
                    break;
                case "decrypt":
                    var decrypted = provider.PromptToDecrypt(argument);
                    Console.WriteLine(Convert.ToBase64String(decrypted));
                    break;
                default:
                    return ExitUsage;
            }
        }
        catch (FormatException)
        {
            return ExitFormat;
        }
        catch (AuthProviderUserCancelledException)
        {
            return ExitUserCancel;
        }
        catch (AuthProviderSystemErrorException)
        {
            return ExitSystem;
        }
        catch (Exception)
        {
            return ExitSystem;
        }

        return 0;
    }
}
