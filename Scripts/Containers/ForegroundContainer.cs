using Godot;
using System;

namespace TOW.Scripts.Containers;

public partial class ForegroundContainer : Control
{
	
	private Control _currentForegroundControl;

	public void ChangeForegroundControl(Control newForegroundControl)
	{
		_currentForegroundControl?.QueueFree();
		_currentForegroundControl = newForegroundControl;
		AddChild(newForegroundControl);
	}
}
