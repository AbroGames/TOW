using Godot;
using KludgeBox.Godot;
using TOW.Scripts.Containers;
using TOW.Scripts.Events;
using TOW.Scripts.Services;

namespace TOW.Scripts;

public partial class Game : Node2D
{
	public override void _Ready()
	{
		var thing = References.Instance.FirstScene;
		var noda = thing.Instantiate(); 
		References.Instance.MenuContainer.ChangeStoredNode(noda as Control);
		ServiceProvider.Get<EventBus>().Publish(new GameReadyEvent(this));
		Audio2D.Setup(this, null);
	}
}
