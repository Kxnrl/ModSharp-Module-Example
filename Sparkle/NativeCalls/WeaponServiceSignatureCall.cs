using Sharp.Shared.Attributes;
using Sharp.Shared.Calls;
using Sharp.Shared.GameObjects;

namespace Kxnrl.Sparkle.NativeCalls;

internal interface IWeaponServiceSignatureCall : ISignatureCall<IWeaponService>
{
    [AddressKey("GetWeaponByName")]
    nint GetWeaponByName(IWeaponService weaponService, string weaponName);
}
