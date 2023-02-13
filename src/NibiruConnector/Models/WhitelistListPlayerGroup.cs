// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Text.Json.Serialization;

namespace NibiruConnector.Models;

public record WhitelistListPlayerGroup
{
    [JsonPropertyName("groupName")]
    public string GroupName { get; set; } = string.Empty;
    
    [JsonPropertyName("players")]    
    public List<WhitelistListPlayer> Players { get; set; } = new();
}
