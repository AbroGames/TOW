using Godot;
using System;
using TOW.Scripts.Containers;
using TOW.Scripts.Content;
using TOW.Scripts.Services;

namespace TOW.Scripts.Screen.Menu.Button;

public partial class SetWorldAndCloseMenuButton : Godot.Button
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
		ServiceProvider.Get<Audio>().PlayUiSound(Sfx.Explosion8Bit);
	}

}
