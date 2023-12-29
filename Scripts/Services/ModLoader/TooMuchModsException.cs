using System;

namespace TOW.Scripts.Services.ModLoader;

public class TooMuchModsException(string message) : Exception(message)
{
    public TooMuchModsException():this("Mod assembly must contain ONLY ONE class derived from Mod"){}
}