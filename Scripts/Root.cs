using Godot;

namespace TOW.Scripts;

/// <summary>
/// Все дети этой ноды будут перемещены в корневую ноду игры.
/// </summary>
public partial class Root : Node
{
	public override void _Ready()
	{
		var root = GetTree().Root;
		
		foreach (var child in GetChildren())
		{
			child.Reparent(root);
		}
		
		QueueFree();
	}

}