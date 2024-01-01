using TOW.Scripts.Services.ModLoader;
using TOW.Scripts.Utils;

namespace ExampleMod;

[StaticConstructorOnStartup]
public static class StaticRunner
{
    static StaticRunner()
    {
        Log.Info("ExampleMod static constructor script executed");
    }
}