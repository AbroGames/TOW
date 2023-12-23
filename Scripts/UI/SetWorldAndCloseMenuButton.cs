using Godot;
using System;
using TOW.Scripts.Containers;

namespace TOW.Scripts.UI;

public partial class SetWorldAndCloseMenuButton : Button
{
	[Export] private PackedScene _newWorldScene;
	
	public override void _Ready()
	{
		Pressed += OnClick;
	}

	private void OnClick()
	{
		References.Instance.WorldContainer.ChangeStoredNode(_newWorldScene.Instantiate() as Node2D);
		References.Instance.MenuContainer.ClearStoredNode();
	}

}
