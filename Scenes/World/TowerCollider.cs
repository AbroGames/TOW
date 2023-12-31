using Godot;
using System;
using TOW.Scripts.World;

public partial class TowerCollider : Area2D
{
	public override void _Ready()
	{
		AreaEntered += OnCollide;
	}

	private void OnCollide(Area2D other)
	{
		var tonk = other.GetParent() as Tank;
		tonk.Hit();
	}
}
