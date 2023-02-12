// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

namespace NibiruConnector.Options;

public record RconOptions
{
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; } = 0;
    public string Password { get; set; } = string.Empty;
}
