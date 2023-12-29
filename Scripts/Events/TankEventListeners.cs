using TOW.Scripts.KludgeBox.Events;
using TOW.Scripts.Utils;
using TOW.Scripts.Utils.Loggers;

namespace TOW.Scripts.Events;

public class TankEventListeners
{
    [EventListener]
    public static void OnTowerRotated(TankTowerRotatedEvent @event)
    {
        Log.Info($"{@event.Tank.Name}'s tower has been rotated");
        @event.Cancel();
    }
    
    [EventListener]
    public static void OnTankMoved(TankMovedEvent @event)
    {
        Log.Info($"{@event.Tank.Name} has been moved");
    }
}