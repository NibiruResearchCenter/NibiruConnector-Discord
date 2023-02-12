// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

namespace NibiruConnector.Interfaces;

public interface IRconService
{
    public Task<string> SendCommand(string command, bool keepMinecraftFormatting = false);
}
