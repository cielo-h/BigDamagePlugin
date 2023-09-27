using Dalamud.Interface.Windowing;
using ImGuiNET;
using System;

namespace BigDamagePlugin.Windows;

public class MainWindow : Window, IDisposable
{
    public MainWindow() : base(
        "BigDamagePlugin", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.AlwaysAutoResize)
    {
    }

    public void Dispose()
    {
    }

    public override void Draw()
    {
        int minvalue = 1; int maxvalue = 100;
        float mult = Plugin.c.Multiplier;
        if (ImGui.Checkbox("Enable", ref Plugin.c.Enabled))
        {
            Plugin.c.Save();
            Plugin.FlyTextHandler.SwitchHook();
        }
        ImGui.Indent(25);
        if (ImGui.Checkbox("Self", ref Plugin.c.Self)) Plugin.c.Save();
        if (ImGui.Checkbox("Party", ref Plugin.c.Party)) Plugin.c.Save();
        if (ImGui.Checkbox("Other", ref Plugin.c.Other)) Plugin.c.Save();
        ImGui.SetNextItemWidth(150);
        if (ImGui.InputFloat($"Multiplier({minvalue}-{maxvalue})", ref mult)) 
        {
            if(mult < minvalue)
            {
                mult = minvalue;
            }
            else if (mult > maxvalue)
            {
                mult = maxvalue;
            }
            Plugin.c.Multiplier = mult;
            Plugin.c.Save();
        };
        ImGui.Unindent();
    }
}
