using System;
using Kxnrl.Sparkle.Interfaces;
using Microsoft.Extensions.Logging;
using Sharp.Extensions.GameEventManager;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace Kxnrl.Sparkle.Modules.EventExample;

internal sealed class ReplaceEvent : IModule
{
    private readonly ILogger<ReplaceEvent> _logger;
    private readonly InterfaceBridge       _bridge;

    public ReplaceEvent(ILogger<ReplaceEvent> logger, InterfaceBridge bridge, IGameEventManager eventManager)
    {
        _logger = logger;
        _bridge = bridge;

        eventManager.HookEvent("player_death", OnPlayerDeath);
        eventManager.HookEvent("weapon_fire",  OnWeaponFire);
    }

    public bool Init()
        => true;

    private HookReturnValue<bool> OnPlayerDeath(IGameEvent e, ref bool serverOnly)
    {
        IGameEvent? clone = null;

        try
        {
            clone = _bridge.EventManager.CloneEvent(e)
                    ?? throw new InvalidOperationException("Failed to clone event");

            // modify kill feed icon
            clone.SetString("weapon", "SawnLake");

            foreach (var client in _bridge.ModSharp.GetIServer().GetGameClients())
            {
                if (client is { SignOnState: SignOnState.Full, IsFakeClient: false })
                {
                    clone.FireToClient(client);
                }
            }

            // hide original event
            serverOnly = true;

            return new HookReturnValue<bool>();
        }
        finally
        {
            clone?.Dispose();
        }
    }

    private HookReturnValue<bool> OnWeaponFire(IGameEvent e, ref bool serverOnly)
    {
        if (_bridge.EventManager.CloneEvent(e) is not { } clone)
        {
            _logger.LogWarning("Failed to clone event {e}", e.Name);

            return new HookReturnValue<bool>();
        }

        clone.SetString("weapon", "weapon_m4a1_silencer");
        clone.FireToClients();
        clone.Dispose();

        // after send clone, we make original event server only
        serverOnly = true;

        return new HookReturnValue<bool>();
    }
}
