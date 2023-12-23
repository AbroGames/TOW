using CodeGenerators;
using Godot;
using Lombok.NET;
using TOW.Scripts.Containers;

namespace TOW.Scripts;

[Singleton]
public partial class References : Node
{
    [Export] public WorldContainer WorldContainer { get; private set; }
    [Export] public BackgroundContainer BackgroundContainer { get; private set; }
    [Export] public HudContainer HudContainer { get; private set; }
    [Export] public MenuContainer MenuContainer { get; private set; }
    [Export] public ForegroundContainer ForegroundContainer { get; private set; }
    [Export] public PackedScene FirstScene { get; private set; }
    
}


