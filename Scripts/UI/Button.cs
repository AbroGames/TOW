using Godot;
using System;

public partial class Button : Godot.Button
{
	[Export] private PackedScene _scene;
	public override void _Ready()
	{
		Pressed += () =>
		{
			var world = _scene.Instantiate();
			Game.Instance.AddChild(world);
			QueueFree(); // Удолить меню
		};
		
	}

}
