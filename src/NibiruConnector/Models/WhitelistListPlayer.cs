// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Text.Json.Serialization;

namespace NibiruConnector.Models;

public record WhitelistListPlayer
{
    [JsonPropertyName("name")]
    public string PlayerName { get; set; } = string.Empty;

    [JsonPropertyName("lastJoin")]
    public long LastJoin { get; set; } = -1;

    [JsonPropertyName("daysSinceLastJoin")]
    public long DaysSinceLastJoin { get; set; } = -1;
}
