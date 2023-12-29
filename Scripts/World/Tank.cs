using Godot;
using TOW.Scripts.Events;
using TOW.Scripts.KludgeBox.Godot.Extensions;
using TOW.Scripts.Services;
using TOW.Scripts.Services.ModLoader;
using TOW.Scripts.Utils;

namespace TOW.Scripts.World;

public partial class Tank : Node2D
{
	[Export] private double _movementSpeed = 250; // in pixels/sec
	[Export] private double _rotationSpeed = 120; // in angles/sec
	[Export] private double _towerRotationSpeed = 240; // in angles/sec
	private Tower Tower => GetNode("Tower") as Tower;
	private EventBus _eventBus => ServiceProvider.Get<EventBus>();

	public override void _Ready()
	{
		
	}

	public override void _Process(double delta)
	{
		var movementInput = GetInput();
		var requiredTowerRotation = GetAimAngle();

		// Movement
		if (movementInput.Y != 0)
		{
			var evt = new TankMovedEvent(this);
			if (!_eventBus.PublishAndCheck(evt))
			{
				Position += this.Up() * movementInput.Y * _movementSpeed * delta;
			}
		}

		// Tank rotation
		if (movementInput.X != 0)
		{
			var evt = new TankRotatedEvent(this);
			if (!_eventBus.PublishAndCheck(evt))
			{
				Rotation += Mathf.DegToRad(movementInput.X * _rotationSpeed * delta);
			}
		}

		// Tower rotation
		var rotationDir = Mathf.Sign(requiredTowerRotation);
		var absoluteRequiredRotation = Mathf.Abs(requiredTowerRotation);

		if (absoluteRequiredRotation > 0.01)
		{
			var evt = new TankTowerRotatedEvent(this);
			if(!_eventBus.PublishAndCheck(evt))
			{
				Tower.Rotation += Mathf.DegToRad(rotationDir * Mathf.Clamp(_towerRotationSpeed * delta, -absoluteRequiredRotation, absoluteRequiredRotation));
			}
		}
	}

	private Vector2 GetInput()
	{
		return Input.GetVector(
			Keys.Left, 
			Keys.Right, 
			Keys.Back, 
			Keys.Forward).Sign();
	}

	private double GetAimAngle()
	{
		var mousePos = GetLocalMousePosition();
		var mouseDir = Tower.Position.DirectionTo(mousePos);
		var mouseAng = mouseDir.Angle();
		var towerAng = Tower.Rotation;

		
		return Mathf.RadToDeg(Mathf.AngleDifference(towerAng - Mathf.Pi/2, mouseAng));
	}
}
