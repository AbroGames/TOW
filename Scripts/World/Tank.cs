using Godot;
using TOW.Scripts.Utils;

namespace TOW.Scripts.World;

public partial class Tank : Node2D
{
	[Export] private double _moveSpeed = 100;
	[Export] private double _rotationSpeed = 10;
	private Tower Tower => GetNode("Tower") as Tower;
	
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
}
