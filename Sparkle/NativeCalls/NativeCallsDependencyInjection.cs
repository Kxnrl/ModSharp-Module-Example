using Microsoft.Extensions.DependencyInjection;

namespace Kxnrl.Sparkle.NativeCalls;

internal static class NativeCallsDependencyInjection
{
    public static IServiceCollection AddNativeCalls(this IServiceCollection services)
        => services
           .AddSingleton<IBaseEntityVCall, BaseEntityVCall>()
           .AddSingleton<IWeaponServiceSignatureCall, WeaponServiceSignatureCall>();
}
