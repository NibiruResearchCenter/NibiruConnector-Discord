// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Text.Json.Serialization;

namespace NibiruConnector.Models;

public record WhitelistListResponse
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; } = 0;

    [JsonPropertyName("playerGroups")]
    public List<WhitelistListPlayerGroup> PlayerGroups { get; set; } = new();
}
