using Godot;
using System;


public partial class Game : Node2D
{
	public static Game Instance { get; private set; }
	public override void _Ready()
	{
		Instance = this;
	}
}
