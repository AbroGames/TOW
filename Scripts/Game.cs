using Godot;
using System;

namespace TOW.Scripts;

public partial class Game : Node2D
{
	
	public override void _Ready()
	{
		References.Instance.MenuContainer.ChangeStoredNode(References.Instance.FirstScene.Instantiate() as Control);
	}
	
	public override void _Process(double delta)
	{
		
	}
}
