using Sharp.Shared.Calls;
using Sharp.Shared.GameEntities;
using Sharp.Shared.Types;

namespace Kxnrl.Sparkle.NativeCalls;

internal interface IBaseEntityVCall : IVirtualCall<IBaseEntity>
{
    unsafe bool PassesDamageFilter(IBaseEntity instance, TakeDamageInfo* pInfo);
}
