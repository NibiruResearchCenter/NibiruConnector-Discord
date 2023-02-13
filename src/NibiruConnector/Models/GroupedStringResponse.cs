// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Text.Json.Serialization;

namespace NibiruConnector.Models;

public record GetGroupResponse
{
    [JsonPropertyName("data")]
    public List<string> Data { get; set; } = new();
}
