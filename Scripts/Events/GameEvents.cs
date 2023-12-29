using TOW.Scripts.KludgeBox.Events;

namespace TOW.Scripts.Events;

public class GameReadyEvent (Game game) : IEvent
{
    public Game Game { get; } = game;
}