using TOW.Scripts.Services.ModLoader;
using TOW.Scripts.Utils;

namespace ExampleMod;

public partial class ExampleMod : Mod
{
    public override void _Ready()
    {
        Log.Info("Example mod is loaded");
    }
}