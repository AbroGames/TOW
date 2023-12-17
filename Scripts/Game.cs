using Godot;
using System;


public partial class Game : Node2D
{
	private Node _currentMainNode;

	public void ChangeMainNode(Node mainNode)
	{
		_currentMainNode?.QueueFree();
		_currentMainNode = mainNode;
		AddChild(mainNode);
	}
	
	public static Game Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
		ChangeMainNode(GD.Load<PackedScene>("res://Scenes/MainMenu.tscn").Instantiate());
	}
}
