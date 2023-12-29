using TOW.Scripts.KludgeBox.Events;
using TOW.Scripts.World;

namespace TOW.Scripts.Events;

public class TankMovedEvent(Tank tank) : CancellableEvent
{
    public Tank Tank { get; } = tank;
}

public class TankTowerRotatedEvent (Tank tank) : CancellableEvent
{
    public Tank Tank { get; } = tank;
}

public class TankRotatedEvent (Tank tank) : CancellableEvent
{
    public Tank Tank { get; } = tank;
}