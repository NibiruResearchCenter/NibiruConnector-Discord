// This file is a part of NibiruConnector.Discord project.
// 
// Copyright (C) 2022 Nibiru Research Center and all Contributors
// 

using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Autocomplete;

namespace NibiruConnector.Commands.AutoComplete;

public class GroupsAutoCompleteProvider : IAutocompleteProvider
{
    private static string[] s_groups = Array.Empty<string>();
    
    public static void UpdateGroups(string[] groups)
    {
        s_groups = groups;
    }
    
    public string Identity => "autocomplete:groups";

    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync(IReadOnlyList<IApplicationCommandInteractionDataOption> options, string userInput, CancellationToken ct = new())
    {
        return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>(s_groups
            .Where(x => x.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(x => new ApplicationCommandOptionChoice(x, x))
            .ToList());
    }
}
