using Dalamud.Game.Gui.FlyText;
using Dalamud.Hooking;
using Dalamud.Logging;
using ECommons.DalamudServices;
using ECommons.GameHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using System;

namespace BigDamagePlugin;

public unsafe class FlyTextHandler
{
    private delegate void flyTextCreationDelegate(
        Character* target,
        Character* source,
        FlyTextKind logKind,
        byte option,
        byte actionKind,
        int actionId,
        int val1,
        int val2,
        byte damageType);
    private readonly Hook<flyTextCreationDelegate>? flyTextCreationHook;

    public FlyTextHandler()
    {
        nint flyTextCreationAddress;

        try
        {
            flyTextCreationAddress = Svc.SigScanner.ScanText("E8 ?? ?? ?? ?? BF ?? ?? ?? ?? EB 39");
        }
        catch (Exception ex)
        {
            Svc.Log.Error(ex, "Sig scan failed.");
            return;
        }
        this.flyTextCreationHook = Svc.Hook.HookFromAddress<flyTextCreationDelegate>(flyTextCreationAddress, this.flyTextCreationDetour);
    }
    internal void SwitchHook()
    {
        if (flyTextCreationHook is null) return;
        bool enabled = Plugin.c.Enabled;
        if (enabled && !flyTextCreationHook.IsEnabled) flyTextCreationHook.Enable();
        if (!enabled && flyTextCreationHook.IsEnabled) flyTextCreationHook.Disable();
    }
    private void flyTextCreationDetour(Character* target, Character* source, FlyTextKind logKind, byte option, byte actionKind, int actionId, int val1, int val2, byte damageType)
    {
        if (ShouldConvert(GetFlyTextCharCategory(source)) && IsShouldConvertkind(logKind))
        {
            float convertvalue = float.Floor(val1 * Plugin.c.Multiplier);
            this.flyTextCreationHook!.Original(target, source, ConvertKind(logKind), option, actionKind, actionId, (int)convertvalue, val2, damageType);
        }
        else { this.flyTextCreationHook!.Original(target, source, logKind, option, actionKind, actionId, val1, val2, damageType); }
    }
    private static FlyTextCharCategory GetFlyTextCharCategory(Character* character)
    {
        var localPlayer = (Character*)(Player.Object?.Address ?? nint.Zero);
        if (character == null || localPlayer == null)
        {
            return FlyTextCharCategory.None;
        }

        if (character == localPlayer)
        {
            return FlyTextCharCategory.Self;
        }

        if (character->IsPartyMember)
        {
            return FlyTextCharCategory.Party;
        }

        return FlyTextCharCategory.Others;
    }
    private bool ShouldConvert(FlyTextCharCategory category)
    {
        switch (category)
        {
            case FlyTextCharCategory.None:
                return false;
            case FlyTextCharCategory.Self:
                if (Plugin.c.Self) { return true; }
                else { return false; }
            case FlyTextCharCategory.Party:
                if (Plugin.c.Party) { return true; }
                else { return false; }
            case FlyTextCharCategory.Others:
                if (Plugin.c.Other) { return true; }
                else { return false; }
            default:
                return false;
        }
    }
    private bool IsShouldConvertkind(FlyTextKind kind)
    {
        switch (kind)
        {
            case FlyTextKind.AutoAttackOrDot:
                return true;
            case FlyTextKind.AutoAttackOrDotCrit:
                return true;
            case FlyTextKind.AutoAttackOrDotDh:
                return true;
            case FlyTextKind.AutoAttackOrDotCritDh:
                return true;
            case FlyTextKind.Damage:
                return true;
            case FlyTextKind.DamageCrit:
                return true;
            case FlyTextKind.DamageDh:
                return true;
            case FlyTextKind.DamageCritDh:
                return true;
            default:
                return false;
        }
    }
    private static FlyTextKind ConvertKind(FlyTextKind kind)
    {
        switch (kind)
        {
            case FlyTextKind.AutoAttackOrDot:
                return FlyTextKind.AutoAttackOrDotCritDh;
            case FlyTextKind.AutoAttackOrDotCrit:
                return FlyTextKind.AutoAttackOrDotCritDh;
            case FlyTextKind.AutoAttackOrDotDh:
                return FlyTextKind.AutoAttackOrDotCritDh;
            case FlyTextKind.Damage:
                return FlyTextKind.DamageCritDh;
            case FlyTextKind.DamageCrit:
                return FlyTextKind.DamageCritDh;
            case FlyTextKind.DamageDh:
                return FlyTextKind.DamageCritDh;
            default:
                return kind;
        }
    }
    public void Dispose()
    {
        this.flyTextCreationHook?.Dispose();
    }

}
