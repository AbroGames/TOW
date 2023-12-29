using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class WorldContainer : Node2D, INodeContainer<Node2D>
{
	public Node2D CurrentStoredNode { get; set; }
	
	public void ChangeStoredNode(Node2D newStoredNode)
	{
		(this as INodeContainer<Node2D>).ChangeStoredNode(newStoredNode);
	}

	public void ClearStoredNode()
	{
		(this as INodeContainer<Node2D>).ClearStoredNode();
	}
}
