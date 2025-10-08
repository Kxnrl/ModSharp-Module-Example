using Kxnrl.Sparkle.Interfaces;
using Kxnrl.Sparkle.Modules.CallAndTraceExample;
using Kxnrl.Sparkle.Modules.EventExample;
using Kxnrl.Sparkle.Modules.HookExample;
using Kxnrl.Sparkle.Modules.SharedInterfaceExample;
using Microsoft.Extensions.DependencyInjection;

namespace Kxnrl.Sparkle.Modules;

internal static class ModulesDependencyInjection
{
    public static IServiceCollection AddModules(this IServiceCollection services)
        => services
           .AddSingleton<IModule, DropAimTargetWeapon>()
           .AddSingleton<IModule, BlockEvent>()
           .AddSingleton<IModule, ListenEvent>()
           .AddSingleton<IModule, ReplaceEvent>()
           .AddSingleton<IModule, FilterCrashFixes>()
           .AddSingleton<IModule, SharedInterface>();
}
