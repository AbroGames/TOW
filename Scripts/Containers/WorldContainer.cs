using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class WorldContainer : Node2D
{
	
	private Node _currentWorldNode;

	public void ChangeWorldNode(Node newWorldNode)
	{
		_currentWorldNode?.QueueFree();
		_currentWorldNode = newWorldNode;
		AddChild(newWorldNode);
	}
}
