using TOW.Scripts.KludgeBox.Events;
using TOW.Scripts.Utils;
using TOW.Scripts.Utils.Loggers;

namespace TOW.Scripts.Events;

public class TankEventListeners
{
    [EventListener(ListenerPriority.Lowest)]
    public static void OnTowerRotatedLowest(TankTowerRotatedEvent @event)
    {
       // Log.Info($"{@event.Tank.Name}'s tower has been rotated (lowest)");
        //@event.Cancel();
    }
    
    [EventListener(ListenerPriority.Monitor)]
    public static void OnTowerRotatedMonitor(TankTowerRotatedEvent @event)
    {
        //Log.Info($"{@event.Tank.Name}'s tower has been rotated (monitor)");
        //@event.Cancel();
    }
    
    [EventListener(ListenerPriority.Highest)]
    public static void OnTowerRotatedHighest(TankTowerRotatedEvent @event)
    {
        //Log.Info($"{@event.Tank.Name}'s tower has been rotated (highest)");
        //@event.Cancel();
    }
    
    [EventListener]
    public static void OnTowerRotatedNormal(TankTowerRotatedEvent @event)
    {
        //Log.Info($"{@event.Tank.Name}'s tower has been rotated (normal)");
        //@event.Cancel();
    }
    
    [EventListener]
    public static void OnTankMoved(TankMovedEvent @event)
    {
        //Log.Info($"{@event.Tank.Name} has been moved");
    }
}