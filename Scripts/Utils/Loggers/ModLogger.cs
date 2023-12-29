using TOW.Scripts.Services.ModLoader;

namespace TOW.Scripts.Utils.Loggers;

internal class ModLogger(Mod mod) : ILogger
{
    internal Mod _mod = mod;
    public void Debug(object msg = null)
    {
        Log.Debug($"[{_mod.ModName}] {msg}");
    }

    public void Info(object msg = null)
    {
        Log.Info($"[{_mod.ModName}] {msg}");
    }

    public void Warning(object msg = null)
    {
        Log.Warning($"[{_mod.ModName}] {msg}");
    }

    public void Error(object msg = null)
    {
        Log.Error($"[{_mod.ModName}] {msg}");
    }

    public void Critical(object msg = null)
    {
        Log.Critical($"[{_mod.ModName}] {msg}");
    }
}