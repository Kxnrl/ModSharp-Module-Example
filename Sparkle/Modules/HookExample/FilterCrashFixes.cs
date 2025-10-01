using System.Runtime.InteropServices;
using Kxnrl.Sparkle.Interfaces;
using Kxnrl.Sparkle.Managers;
using Kxnrl.Sparkle.Managers.Hook;
using Microsoft.Extensions.Logging;

namespace Kxnrl.Sparkle.Modules.HookExample;

internal sealed class FilterCrashFixes : IModule
{
    private readonly ILogger<FilterCrashFixes> _logger;
    private readonly InterfaceBridge           _bridge;
    private readonly IHookManager              _hookManager;

    public FilterCrashFixes(ILogger<FilterCrashFixes> logger, InterfaceBridge bridge, IHookManager hookManager)
    {
        _logger      = logger;
        _bridge      = bridge;
        _hookManager = hookManager;
    }

    public bool Init()
        => true;

    private static unsafe delegate* unmanaged<nint, InputData*, void> _sTrampoline;

    public unsafe void OnPostInit()
    {
        var hook = _hookManager.CreateDetour("CBaseFilter",
                                             "InputTestActivator",
                                             (nint) (delegate* unmanaged<nint, InputData*, void>) (&Hook));

        if (hook.Install())
        {
            _sTrampoline = (delegate* unmanaged<nint, InputData*, void>) hook.Trampoline;
        }
    }

    [UnmanagedCallersOnly]
    private static unsafe void Hook(nint pEntity, InputData* pInput)
    {
        if (pInput->pActivator == nint.Zero)
        {
            return;
        }

        _sTrampoline(pEntity, pInput);
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputData
    {
        [FieldOffset(0)]
        public nint pActivator;

        [FieldOffset(8)]
        public nint pCaller;
    }
}
