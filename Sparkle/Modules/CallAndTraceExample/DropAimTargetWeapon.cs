using Kxnrl.Sparkle.Interfaces;
using Kxnrl.Sparkle.Modules.EventExample;
using Kxnrl.Sparkle.NativeCalls;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Definition;
using Sharp.Shared.Enums;
using Sharp.Shared.GameEntities;
using Sharp.Shared.HookParams;
using Sharp.Shared.Types;

namespace Kxnrl.Sparkle.Modules.CallAndTraceExample;

internal class DropAimTargetWeapon : IModule
{
    private readonly ILogger<BlockEvent>         _logger;
    private readonly InterfaceBridge             _bridge;
    private readonly IWeaponServiceSignatureCall _weaponServiceCall;
    private readonly IBaseEntityVCall            _entityCall;

    public DropAimTargetWeapon(InterfaceBridge bridge,
        ILogger<BlockEvent>                    logger,
        IWeaponServiceSignatureCall            weaponServiceCall,
        IBaseEntityVCall                       entityCall)
    {
        _logger            = logger;
        _bridge            = bridge;
        _weaponServiceCall = weaponServiceCall;
        _entityCall        = entityCall;
    }

    public bool Init()
        => true;

    public void OnPostInit()
    {
        _bridge.HookManager.PlayerRunCommand.InstallHookPost(OnPlayerRunCommand);
    }

    public void Shutdown()
    {
        _bridge.HookManager.PlayerRunCommand.RemoveHookPost(OnPlayerRunCommand);
    }

    private void OnPlayerRunCommand(IPlayerRunCommandHookParams param, HookReturnValue<EmptyHookReturn> result)
    {
        if (result.Action >= EHookAction.SkipCallReturnOverride)
        {
            return;
        }

        if (!param.Pawn.IsAlive || param.Pawn.GetWeaponBySlot(GearSlot.Knife, 0) is not { } knife)
        {
            return;
        }

        var startPos = param.Pawn.GetEyePosition();
        var eyeAgl   = param.Pawn.GetEyeAngles();
        var endPos   = startPos + (eyeAgl.AnglesToVectorForward() * 2048);

        var trace = _bridge.PhysicsQuery.TraceLine(startPos,
                                                   endPos,
                                                   UsefulInteractionLayers.FireBullets,
                                                   CollisionGroupType.Default,
                                                   TraceQueryFlag.All,
                                                   InteractionLayers.None,
                                                   param.Pawn);

        if (trace.HitEntity?.AsPlayerPawn() is not { IsAlive: true } target)
        {
            return;
        }

        if (target.GetWeaponService() is not { } ws)
        {
            return;
        }

        var info = TakeDamageInfo.CreateFromSlash(param.Pawn, knife);

        unsafe
        {
            if (!_entityCall.PassesDamageFilter(target, &info))
            {
                // target is immune to slash damage from the attacker
                return;
            }
        }

        var pWeapon = _weaponServiceCall.GetWeaponByName(ws, "weapon_knife");

        if (_bridge.ModSharp.CreateNativeObject<IBaseWeapon>(pWeapon) is not { } weapon)
        {
            return;
        }

        target.DropWeapon(weapon);
    }
}
