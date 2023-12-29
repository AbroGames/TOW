using Godot;
using TOW.Scripts.KludgeBox.Events;

namespace TOW.Scripts.Events;

public class WorldChangedEvent (Node2D world, Node2D newWorld) : CancellableEvent
{
    public Node2D World { get; } = world;
}

public class WorldRemovedEvent(Node2D world) : CancellableEvent
{
    public Node2D World { get; } = world;
}