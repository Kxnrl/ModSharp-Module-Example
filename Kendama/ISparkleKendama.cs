using Sharp.Shared.GameEntities;
using Sharp.Shared.Objects;

namespace Kxnrl.Sparkle.Kendama;

public interface ISparkleKendama
{
    static string Identity => typeof(ISparkleKendama).FullName ?? nameof(ISparkleKendama);

    void Hello(IGameClient client);

    void Idle(IPlayerController controller);

    void Kick(IPlayerPawn pawn);
}
