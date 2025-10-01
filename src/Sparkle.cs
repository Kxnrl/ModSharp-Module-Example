using System;
using System.IO;
using Kxnrl.Sparkle.Extensions;
using Kxnrl.Sparkle.Interfaces;
using Kxnrl.Sparkle.Managers;
using Kxnrl.Sparkle.Modules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Shared;

namespace Kxnrl.Sparkle;

public sealed class Sparkle : IModSharpModule
{
    public string DisplayName   => "Sparkle from StarRail";
    public string DisplayAuthor => "Kxnrl";

    private readonly ILogger<Sparkle> _logger;
    private readonly InterfaceBridge  _bridge;
    private readonly ServiceProvider  _serviceProvider;

    public Sparkle(ISharedSystem sharedSystem,
        string?                  dllPath,
        string?                  sharpPath,
        Version?                 version,
        IConfiguration?          coreConfiguration,
        bool                     hotReload)
    {
        ArgumentNullException.ThrowIfNull(dllPath);
        ArgumentNullException.ThrowIfNull(sharpPath);
        ArgumentNullException.ThrowIfNull(version);
        ArgumentNullException.ThrowIfNull(coreConfiguration);

        var bridge = new InterfaceBridge(dllPath, sharpPath, version, sharedSystem);

        var configuration = new ConfigurationBuilder()
                            .AddJsonFile(Path.Combine(dllPath, "appsettings.json"), false, false)
                            .Build();

        sharedSystem.GetModSharp().GetGameData().Register("Sparkle.games");

        var services = new ServiceCollection();

        services.AddSingleton(bridge);
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(sharedSystem.GetLoggerFactory());

        ConfigureServices(services);

        _bridge          = new InterfaceBridge(dllPath, sharpPath, version, sharedSystem);
        _logger          = sharedSystem.GetLoggerFactory().CreateLogger<Sparkle>();
        _serviceProvider = services.BuildServiceProvider();
    }

    public bool Init()
    {
        _logger.LogInformation(
            "Oh wow, we seem to be crossing paths a lot lately... Where could I have seen you before? Can you figure it out?");

        var init = 0;

        foreach (var service in _serviceProvider.GetServices<IManager>())
        {
            if (!service.Init())
            {
                _logger.LogError("Failed to init manager {service}!", service.GetType().FullName);

                return false;
            }

            init++;
        }

        if (init == 0)
        {
            throw new ApplicationException("No Modules");
        }

        CallPostInit<IManager>();
        CallPostInit<IModule>();

        return true;
    }

    public void Shutdown()
    {
        _logger.LogInformation("See you around, Nameless~ Try to stay out of trouble, especially... the next time we meet!");

        CallShutdown<IModule>();
        CallShutdown<IManager>();

        // You must unregister your game data when your module is unloaded.
        _bridge.GameData.Unregister("Sparkle.games");
    }

    public void PostInit()
    {
        _logger.LogInformation("Why don't you stay and play for a while?");
    }

    public void OnAllModulesLoaded()
    {
        _logger.LogInformation("A foolish sage or a wise fool... Who will I become next?");
    }

    public void OnLibraryConnected(string name)
    {
        _logger.LogInformation("The~ Game~ Is~ On~");
    }

    public void OnLibraryDisconnect(string name)
    {
        _logger.LogInformation("Done playing for today...");
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services
            .AddManagers() // Managers
            .AddModules(); // Modules;
    }

    private void CallPostInit<T>() where T : IBaseInterface
    {
        foreach (var service in _serviceProvider.GetServices<T>())
        {
            try
            {
                service.OnPostInit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while calling PostInit in {m}", service.GetType().Name);
            }
        }
    }

    private void CallShutdown<T>() where T : IBaseInterface
    {
        foreach (var service in _serviceProvider.GetServices<T>())
        {
            try
            {
                service.Shutdown();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while calling Shutdown in {m}", service.GetType().Name);
            }
        }
    }
}
