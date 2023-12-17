using CodeGenerators;
using Godot;
using Lombok.NET;

namespace TOW.Scripts;


// Simple singleton description class
[GdSingleton]
public partial class References : Node
{
    [Export] private PackedScene _scene;
    [Export] private Vector2 _position;
    [Export] private string _name;

}


