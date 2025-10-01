using Kxnrl.Sparkle.Interfaces;
using Kxnrl.Sparkle.Modules.HookExample;
using Microsoft.Extensions.DependencyInjection;

namespace Kxnrl.Sparkle.Modules;

internal static class ModulesDependencyInjection
{
    public static IServiceCollection AddModules(this IServiceCollection services)
    {
        services.AddSingleton<IModule, FilterCrashFixes>();

        return services;
    }
}
