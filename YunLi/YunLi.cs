using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.GameEvents;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace Kxnrl.YunLi;

public sealed class YunLi : IModSharpModule, IEventListener, IEntityListener
{
    public string DisplayName   => "YunLi from StarRail";
    public string DisplayAuthor => "Kxnrl";

    private readonly ILogger<YunLi> _logger;
    private readonly ISharedSystem  _shared;

    public YunLi(ISharedSystem sharedSystem,
        string?                dllPath,
        string?                sharpPath,
        Version?               version,
        IConfiguration?        coreConfiguration,
        bool                   hotReload)
    {
        _logger = sharedSystem.GetLoggerFactory().CreateLogger<YunLi>();
        _shared = sharedSystem;
    }

    public bool Init()
        => true;

    public void PostInit()
    {
        // on server console only
        _shared.GetConVarManager()
               .CreateServerCommand("ms_echo",
                                    OnServerCommand,
                                    "Command Description",
                                    ConVarFlags.Release);

        // client chat/console
        _shared.GetClientManager().InstallCommandCallback("hello", OnClientCommand);

        _shared.GetEventManager().InstallEventListener(this);
    }

    public void Shutdown()
    {
        _shared.GetConVarManager().ReleaseCommand("ms_echo");
        _shared.GetClientManager().RemoveCommandCallback("hello", OnClientCommand);

        _shared.GetEventManager().RemoveEventListener(this);
    }

#region Command

    // type 'ms_econ' on server console
    private ECommandAction OnServerCommand(StringCommand command)
    {
        Console.WriteLine("Hello");
        _shared.GetModSharp().LogMessage($"Trigger command {command.GetCommandString()}");

        return ECommandAction.Stopped;
    }

    // type 'ms_hello' in client console, or chat trigger with '.hello' or '/hello' or '!hello'
    private ECommandAction OnClientCommand(IGameClient client, StringCommand command)
    {
        var name = command.ArgCount > 0 ? command.GetArg(1) : "YunLi";

        // console via engine client
        client.ConsolePrint($"Hello, {name}");

        // text message via server.dll
        _shared.GetModSharp()
               .PrintChannelFilter(HudPrintChannel.Chat,
                                   $"Hello, {name}",
                                   new RecipientFilter(client.Slot));

        _logger.LogInformation("OnClientCommand -> {client}: {command}", client.Name, command.GetCommandString());

        return ECommandAction.Stopped;
    }

#endregion

#region Event

    public void FireGameEvent(IGameEvent e)
    {
        if (e is IEventPlayerDeath death)
        {
            _logger.LogInformation("{v} was killed by {k}",
                                   death.VictimController?.PlayerName,
                                   death.KillerController?.PlayerName ?? "World");
        }
        else if (e.Name.Equals("player_spawn"))
        {
            _logger.LogInformation("Player slot[{s}] spawned", e.GetInt("userid"));
        }
        else
        {
            _logger.LogInformation("GameEvent {e} fired", e.Name);
        }
    }

    // by default, Event hook implement isn't needed.
    public bool HookFireEvent(IGameEvent e, ref bool serverOnly)
    {
        if (e.Name.Equals("player_say", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Blocked GameEvent fire: {s}", e.Name);

            // block event
            return false;
        }

        return true;
    }

    int IEventListener.ListenerVersion => IEventListener.ApiVersion;

    int IEventListener.ListenerPriority => 0;

#endregion

#region MyRegion

    public void OnEntitySpawned(IBaseEntity entity)
    {
        _logger.LogDebug("OnEntityCreated<{class}> at {index}", entity.Classname, entity.Index);
    }

    public void OnEntityDeleted(IBaseEntity entity)
    {
        _logger.LogDebug("OnEntityDeleted<{class}> at {index}", entity.Classname, entity.Index);
    }

    // You can implement more entity event here.

    int IEntityListener.ListenerVersion => IEntityListener.ApiVersion;

    int IEntityListener.ListenerPriority => 0;

#endregion
}
