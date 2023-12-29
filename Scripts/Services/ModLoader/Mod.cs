using Godot;

namespace TOW.Scripts.Services.ModLoader;

public abstract partial class Mod : Node
{
    public string ModName { get; protected set; }
}