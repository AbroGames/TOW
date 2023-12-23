using Godot;
using System;

namespace TOW.Scripts;

public partial class Game : Node2D
{
	private Node _currentWorldNode;
	private Node _currentScreenNode;
	
	public override void _Ready()
	{
		
	}
	
	public void ChangeWorldNode(Node newWorldNode)
	{
		_currentWorldNode?.QueueFree();
		_currentWorldNode = newWorldNode;
		AddChild(newWorldNode);
	}
}
