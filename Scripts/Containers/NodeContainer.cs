using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class NodeContainer <TStored> : Node where TStored : Node
{
	
	private TStored _currentStoredNode;

	public void ChangeStoredNode(TStored newStoredNode)
	{
		_currentStoredNode?.QueueFree();
		_currentStoredNode = newStoredNode;
		AddChild(newStoredNode);
	}

	public void ClearStoredNode()
	{
		_currentStoredNode?.QueueFree();
	}
}
