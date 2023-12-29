using Godot;

namespace TOW.Scripts.Services;


[GlobalClass]
public abstract partial class Service : Node
{
    internal abstract void Run();
}