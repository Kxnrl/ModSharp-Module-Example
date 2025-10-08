using Kxnrl.Sparkle.Interfaces;
using Microsoft.Extensions.Logging;
using Sharp.Extensions.GameEventManager;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEvents;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace Kxnrl.Sparkle.Modules.EventExample;

internal sealed class BlockEvent : IModule
{
    private readonly ILogger<BlockEvent> _logger;

    public BlockEvent(ILogger<BlockEvent> logger, IGameEventManager eventManager)
    {
        _logger = logger;

        eventManager.HookEvent("player_changename", OnPlayerChangeName);
        eventManager.HookEvent("weapon_reload",     OnWeaponReload);
    }

    public bool Init()
        => true;

    private HookReturnValue<bool> OnPlayerChangeName(IGameEvent ev, ref bool serverOnly)
    {
        // [[unlikely]] this should never happen
        if (ev is not IEventPlayerChangeName e)
        {
            return new HookReturnValue<bool>();
        }

        _logger.LogInformation("{old} changed name to {new}", e.OldName, e.NewName);

        // block event fire
        return new HookReturnValue<bool>(EHookAction.SkipCallReturnOverride);
    }

    private static HookReturnValue<bool> OnWeaponReload(IGameEvent e, ref bool serverOnly)
    {
        // make server only,
        // don't broadcast to client
        serverOnly = true;

        return new HookReturnValue<bool>();
    }
}
