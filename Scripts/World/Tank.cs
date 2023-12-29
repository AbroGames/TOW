using Godot;
using TOW.Scripts.KludgeBox.Godot.Extensions;
using TOW.Scripts.Utils;

namespace TOW.Scripts.World;

public partial class Tank : Node2D
{
	[Export] private double _movementSpeed = 250; // in pixels/sec
	[Export] private double _rotationSpeed = 120; // in angles/sec
	[Export] private double _towerRotationSpeed = 240; // in angles/sec
	private Tower Tower => GetNode("Tower") as Tower;
	
	
	public override void _Process(double delta)
	{
		var movementInput = GetInput();
		var requiredTowerRotation = GetAimAngle();

		// Movement
		Position += this.Up() * -movementInput.Y * _movementSpeed * delta;

		// Tank rotation
		Rotation += Mathf.DegToRad(movementInput.X * _rotationSpeed * delta);

		// Tower rotation
		var rotationDir = Mathf.Sign(requiredTowerRotation);
		var absoluteRequiredRotation = Mathf.Abs(requiredTowerRotation);
		Tower.Rotation += Mathf.DegToRad(rotationDir * Mathf.Clamp(_towerRotationSpeed * delta, -absoluteRequiredRotation, absoluteRequiredRotation));
	}

	private Vector2 GetInput()
	{
		return Input.GetVector(
			Keys.Left, 
			Keys.Right, 
			Keys.Forward, 
			Keys.Back).Sign();
	}

	private double GetAimAngle()
	{
		var mousePos = GetLocalMousePosition();
		var mouseDir = Tower.Position.DirectionTo(mousePos);
		var mouseAng = mouseDir.Angle();
		var towerAng = Tower.Rotation;

		
		return Mathf.RadToDeg(Mathf.AngleDifference(towerAng, mouseAng) + Mathf.Pi/2);
	}
}
