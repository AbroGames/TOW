using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class BackgroundContainer : Control
{
	
	private Control _currentBackgroundControl;

	public void ChangeBackgroundControl(Control newBackgroundControl)
	{
		_currentBackgroundControl?.QueueFree();
		_currentBackgroundControl = newBackgroundControl;
		AddChild(newBackgroundControl);
	}
}
