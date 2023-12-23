using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class MenuContainer : Control
{
	
	private Control _currentMenuControl;

	public void ChangeMenuControl(Control newMenuControl)
	{
		_currentMenuControl?.QueueFree();
		_currentMenuControl = newMenuControl;
		AddChild(newMenuControl);
	}
}
