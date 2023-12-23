using Godot;
using System;

namespace TOW.Scripts;

public partial class Game : Node2D
{
	public override void _Ready()
	{
		var thing = References.Instance.FirstScene;
		var noda = thing.Instantiate();
		References.Instance.MenuContainer.ChangeStoredNode(noda as Control);
	}
	
	public override void _Process(double delta)
	{
		
	}
}
