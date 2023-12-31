using TOW.Scripts.KludgeBox.Collections;

namespace TOW.Scripts.Content;

internal static class Sfx
{
    internal const string SoundsDir = "res://Assets/Audio/Sounds";
		
    // impacts
    internal static RandomPicker<string> Hit { get; } = new RandomPicker<string>(
        $"{SoundsDir}/MetalHeavyBashMetal{{0}}.wav".BatchNumber(1, 3)
    );
}