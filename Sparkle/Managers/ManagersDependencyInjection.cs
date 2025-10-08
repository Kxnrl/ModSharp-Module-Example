using Kxnrl.Sparkle.Extensions;
using Kxnrl.Sparkle.Interfaces;
using Kxnrl.Sparkle.Managers.Hook;
using Microsoft.Extensions.DependencyInjection;

namespace Kxnrl.Sparkle.Managers;

internal static class ManagersDependencyInjection
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
        => services
            .AddSingleton<IManager, IHookManager, HookManager>();
}
