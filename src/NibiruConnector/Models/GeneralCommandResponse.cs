// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using System.Text.Json.Serialization;

namespace NibiruConnector.Models;

public record GeneralCommandResponse
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; } = 0;
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
