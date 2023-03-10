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
    private static List<string> s_groups = new();
    
    public static void UpdateGroups(List<string> groups)
    {
        s_groups = groups;
    }

    public string Identity => AutoCompleteIdentities.GROUPS;

    public ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>> GetSuggestionsAsync(IReadOnlyList<IApplicationCommandInteractionDataOption> options, string userInput, CancellationToken ct = new())
    {
        return new ValueTask<IReadOnlyList<IApplicationCommandOptionChoice>>(s_groups
            .Where(x => x.StartsWith(userInput, StringComparison.OrdinalIgnoreCase))
            .Take(25)
            .Select(x => new ApplicationCommandOptionChoice(x, x))
            .ToList());
    }
}
