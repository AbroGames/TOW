using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class HudContainer : Control
{
	
	private Control _currentHudControl;

	public void ChangeHudControl(Control newHudControl)
	{
		_currentHudControl?.QueueFree();
		_currentHudControl = newHudControl;
		AddChild(newHudControl);
	}
}
