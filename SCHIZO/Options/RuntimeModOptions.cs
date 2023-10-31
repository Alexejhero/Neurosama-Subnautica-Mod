﻿using System.Collections.Generic;
using Nautilus.Options;

namespace SCHIZO.Options;

public sealed class RuntimeModOptions : ModOptions
{
    public RuntimeModOptions(List<ModOption> options) : base("Neuro-sama Mod")
    {
        foreach (ModOption option in options)
        {
            AddItem(option.GetOptionItem());
        }

        GameObjectCreated += OnGameObjectCreated;
    }

    private void OnGameObjectCreated(object _, GameObjectCreatedEventArgs evt)
    {
        ModOption modOption = ModOption.OptionItems[_options[evt.Id]];
        modOption.AddRealtimeUpdater(evt.Value);
    }
}
