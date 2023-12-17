using Godot;
using System;

public partial class Button : Godot.Button
{
	[Export] private PackedScene _scene;
	
	public override void _Ready()
	{
		Pressed += () =>
		{
			Game.Instance.ChangeMainNode(_scene.Instantiate());
		};
		
	}

}
