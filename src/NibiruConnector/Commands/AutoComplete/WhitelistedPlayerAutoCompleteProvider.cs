// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace NibiruConnector.Commands.AutoComplete;

public class WhitelistedPlayerAutoCompleteProvider : IAutocompleteProvider
{
    private static List<string> s_whitelistedPlayers = new();
    
    public static void UpdateWhitelistedPlayers(List<string> whitelistedPlayers)
    {
        s_whitelistedPlayers = whitelistedPlayers;
    }
    
    public static void AddWhitelistedPlayer(string whitelistedPlayer)
    {
        s_whitelistedPlayers.Add(whitelistedPlayer);
    }
    
    public static void RemoveWhitelistedPlayer(string whitelistedPlayer)
    {
        s_whitelistedPlayers.Remove(whitelistedPlayer);
    }

    public string Identity => AutoCompleteIdentities.WHITELISTED_PLAYERS;

    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync(IReadOnlyList<IApplicationCommandInteractionDataOption> options, string userInput, CancellationToken ct = new())
    {
        return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>(s_whitelistedPlayers
            .Where(x => x.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(x => new ApplicationCommandOptionChoice(x, x))
            .ToList());
    }
}
