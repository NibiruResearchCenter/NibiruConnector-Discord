// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

namespace NibiruConnector.Options;

public record DiscordOptions
{
    public string Token { get; set; } = string.Empty;
    public ulong GuildId { get; set; } = 0;
}
