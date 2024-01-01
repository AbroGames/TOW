using TOW.Scripts.KludgeBox.Collections;

namespace TOW.Scripts.Content;

internal static class Sfx
{
    internal const string SoundsDir = "res://Assets/Audio/Sounds";
		
    internal static RandomPicker<string> Hit { get; } = new RandomPicker<string>(
        $"{SoundsDir}/MetalHeavyBashMetal{{0}}.wav".BatchNumber(1, 3)
    );
    
    internal static RandomPicker<string> Woosh { get; } = new RandomPicker<string>(
        $"{SoundsDir}/woosh_short{{0}}.wav".BatchNumber(1, 5)
    );
}