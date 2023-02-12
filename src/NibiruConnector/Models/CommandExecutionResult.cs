// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

namespace NibiruConnector.Models;

public record CommandExecutionResult<T>(string Command, string Response, long TimeConsumption)
{
    public T? Result { get; set; }
}
