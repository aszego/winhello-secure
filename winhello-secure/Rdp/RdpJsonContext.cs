// winhello-secure - part of winhello-secure (GPL-3.0)

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhSecure.Rdp;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Dictionary<string, RdpEntry>))]
internal sealed partial class RdpJsonContext : JsonSerializerContext
{
}
