using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class MenuContainer : Control, INodeContainer<Control>
{
	public Control CurrentStoredNode { get; set; }
	
	public void ChangeStoredNode(Control newStoredNode)
	{
		(this as INodeContainer<Control>).ChangeStoredNode(newStoredNode);
	}

	public void ClearStoredNode()
	{
		(this as INodeContainer<Control>).ClearStoredNode();
	}
}
