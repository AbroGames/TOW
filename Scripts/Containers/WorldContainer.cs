using Godot;
using System;
using TOW.Scripts.Events;
using TOW.Scripts.Services;

namespace TOW.Scripts.Containers;

public partial class WorldContainer : Node2D, INodeContainer<Node2D>
{
	public Node2D CurrentStoredNode { get; set; }
	
	public void ChangeStoredNode(Node2D newStoredNode)
	{
		if (ServiceProvider.Get<EventBus>().PublishAndCheck(new WorldChangedEvent(CurrentStoredNode, newStoredNode))) return;
		
		CurrentStoredNode?.QueueFree();
		CurrentStoredNode = newStoredNode;
		(this as Node)?.AddChild(newStoredNode);
	}

	public void ClearStoredNode()
	{
		if (ServiceProvider.Get<EventBus>().PublishAndCheck(new WorldRemovedEvent(CurrentStoredNode))) return;
		
		CurrentStoredNode?.QueueFree();
	}
}
