using Godot;
using System;

namespace TOW.Scripts.UI;

public partial class Button : Godot.Button
{
	[Export] private PackedScene _scene;
	
	public override void _Ready()
	{
		Pressed += () =>
		{
			//Game.Instance.ChangeMainNode(_scene.Instantiate());
			GetParent().GetParent().QueueFree();
		};
		
	}

}
