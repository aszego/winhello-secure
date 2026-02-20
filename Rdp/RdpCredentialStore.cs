using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WhSecure.Rdp;

internal static class RdpCredentialStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    private static string StorePath =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WhSecure",
            "rdp-credentials.json");

    public static string NormalizePath(string rdpPath)
    {
        return Path.GetFullPath(Path.GetFullPath(rdpPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    }

    public static void Upsert(string normalizedRdpPath, string username, string encryptedPasswordBase64)
    {
        var dir = Path.GetDirectoryName(StorePath)!;
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var store = Load();
        store[normalizedRdpPath] = new RdpEntry(username, encryptedPasswordBase64);
        Save(store);
    }

    public static RdpEntry? Get(string normalizedRdpPath)
    {
        var store = Load();
        return store.TryGetValue(normalizedRdpPath, out var entry) ? entry : null;
    }

    private static Dictionary<string, RdpEntry> Load()
    {
        if (!File.Exists(StorePath))
            return new Dictionary<string, RdpEntry>();

        try
        {
            var json = File.ReadAllText(StorePath);
            var dict = JsonSerializer.Deserialize<Dictionary<string, RdpEntry>>(json);
            return dict ?? new Dictionary<string, RdpEntry>();
        }
        catch
        {
            return new Dictionary<string, RdpEntry>();
        }
    }

    private static void Save(Dictionary<string, RdpEntry> store)
    {
        var json = JsonSerializer.Serialize(store, JsonOptions);
        File.WriteAllText(StorePath, json);
    }
}
