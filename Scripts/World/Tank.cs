using Godot;
using KludgeBox.Godot;
using TOW.Scripts.Content;
using TOW.Scripts.Events;
using TOW.Scripts.KludgeBox.Godot.Extensions;
using TOW.Scripts.KludgeBox.Scheduling;
using TOW.Scripts.Services;
using TOW.Scripts.Services.ModLoader;
using TOW.Scripts.Utils;

namespace TOW.Scripts.World;

public partial class Tank : Node2D
{
	[Export] private double _movementSpeed = 250; // in pixels/sec
	[Export] private double _rotationSpeed = 120; // in angles/sec
	[Export] private double _towerRotationSpeed = 240; // in angles/sec
	[Export] private bool _isPlayer = true;
	private Tower Tower => GetNode("Tower") as Tower;
	private Sprite2D Sprite => GetNode("Sprite2D") as Sprite2D;
	private EventBus _eventBus => ServiceProvider.Get<EventBus>();
	private bool _dashing = false;

	private Cooldown _dashCd = new Cooldown(0.2, CooldownMode.Single);
	private Cooldown _dashDuration = new Cooldown(0.5, CooldownMode.Single);
	private double HitOpacity
	{
		get => (Sprite.Material as ShaderMaterial)?.GetShaderParameter("mixAmount").AsDouble() ?? -1;
		set => (Sprite.Material as ShaderMaterial)?.SetShaderParameter("mixAmount", value);
	}
	
	public override void _Ready()
	{
		_dashDuration.OnReady += () =>
		{
			_dashing = false;
		};
	}

	public override void _Process(double delta)
	{
		HitOpacity = Mathf.Max(HitOpacity - 0.02, 0);
		_dashCd.Update(delta);
		_dashDuration.Update(delta);
		if(!_isPlayer) return;
		
		var movementInput = GetInput();
		var requiredTowerRotation = GetAimAngle();

		
		// Movement
		if (movementInput.Y != 0)
		{
			var evt = new TankMovedEvent(this);
			var speed = _dashing ? _movementSpeed * 3 : _movementSpeed;
			
			if (!_eventBus.PublishAndCheck(evt))
			{
				Position += this.Up() * movementInput.Y * speed * delta;
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

	public void Hit()
	{
		HitOpacity = 1;
		Audio2D.PlaySoundOn((string)Sfx.Hit, this, 1);
	}

	private int _num = 0;
	public override void _Input(InputEvent @event)
	{
		if(Input.IsActionJustPressed(Keys.Forward))
		{
			if (_dashCd.FractionElapsed is < 1 and > 0)
			{
				_dashDuration.Restart();
				_dashing = true;
			}
			else
			{
				_dashCd.Restart();
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
