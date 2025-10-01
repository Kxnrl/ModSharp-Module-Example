using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Enums;
using Sharp.Shared.Objects;
using Sharp.Shared.Types;

namespace Kxnrl.YunLi;

public sealed class YunLi : IModSharpModule
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
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);

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
    }

    public void Shutdown()
    {
        _shared.GetConVarManager().ReleaseCommand("ms_echo");
        _shared.GetClientManager().RemoveCommandCallback("hello", OnClientCommand);
    }

    // type 'ms_econ' on server console
    private ECommandAction OnServerCommand(StringCommand arg)
    {
        Console.WriteLine("Hello");
        _shared.GetModSharp().LogMessage($"Trigger command {arg.GetCommandString()}");

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

        return ECommandAction.Stopped;
    }
}
