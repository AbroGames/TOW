using System;
using System.Collections.Generic;
using Godot;
using TOW.Scripts.KludgeBox.Collections;
using TOW.Scripts.KludgeBox.Godot.Extensions;

namespace TOW.Scripts.Services;

[GlobalClass]
public partial class Audio : Service
{
    
    private HashSet<AudioStreamPlayer> _uiSounds = new HashSet<AudioStreamPlayer>();
    private HashSet<AudioStreamPlayer2D> _worldSounds = new HashSet<AudioStreamPlayer2D>();
    private Node2D _game;
    
    
    /// <summary>
    /// Name of the master bus.
    /// </summary>
    protected string MasterBus { get; } = "Master";

    /// <summary>
    /// Name of the music bus.
    /// </summary>
    protected string MusicBus { get; } = "Music";

    /// <summary>
    /// Name of the sounds bus.
    /// </summary>
    protected string SoundsBus { get; } = "Sounds";

    /// <summary>
    /// Index of the master bus.
    /// </summary>
    protected virtual int MasterIndex => AudioServer.GetBusIndex(MasterBus);

    /// <summary>
    /// Index of the music bus.
    /// </summary>
    protected virtual int MusicIndex => AudioServer.GetBusIndex(MusicBus);

    /// <summary>
    /// Index of the sounds bus.
    /// </summary>
    protected virtual int SoundsIndex => AudioServer.GetBusIndex(SoundsBus);
    
    /// <summary>
    /// The currently playing music stream player.
    /// </summary>
    public virtual AudioStreamPlayer CurrentMusic { get; private set; } = null;

    /// <summary>
    /// Read-only collection of currently playing UI sounds.
    /// </summary>
    public virtual ReadOnlyHashSet<AudioStreamPlayer> PlayingUiSounds => _uiSounds.AsReadOnly();

    /// <summary>
    /// Read-only collection of currently playing world sounds.
    /// </summary>
    public virtual ReadOnlyHashSet<AudioStreamPlayer2D> PlayingWorldSounds => _worldSounds.AsReadOnly();
    
    /// <summary>
    /// Master volume in linear scale (0.0 to 1.0).
    /// </summary>
    public virtual float MasterVolume
    {
        get => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(MasterIndex));
        set => AudioServer.SetBusVolumeDb(MasterIndex, Mathf.LinearToDb(value));
    }

    /// <summary>
    /// Master volume in decibels.
    /// </summary>
    public virtual float MasterVolumeDb
    {
        get => AudioServer.GetBusVolumeDb(MasterIndex);
        set => AudioServer.SetBusVolumeDb(MasterIndex, value);
    }

    /// <summary>
    /// Music volume in linear scale (0.0 to 1.0).
    /// </summary>
    public virtual float MusicVolume
    {
        get => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(MusicIndex));
        set => AudioServer.SetBusVolumeDb(MusicIndex, Mathf.LinearToDb(value));
    }
    
    /// <summary>
    /// Music volume in decibels.
    /// </summary>
    public virtual float MusicVolumeDb
    {
        get => AudioServer.GetBusVolumeDb(MusicIndex);
        set => AudioServer.SetBusVolumeDb(MusicIndex, value);
    }

    /// <summary>
    /// Sounds volume in linear scale (0.0 to 1.0).
    /// </summary>
    public virtual float SoundsVolume
    {
        get => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(SoundsIndex));
        set => AudioServer.SetBusVolumeDb(SoundsIndex, Mathf.LinearToDb(value));
    }

    /// <summary>
    /// Sounds volume in decibels.
    /// </summary>
    public virtual float SoundsVolumeDb
    {
        get => AudioServer.GetBusVolumeDb(SoundsIndex);
        set => AudioServer.SetBusVolumeDb(SoundsIndex, value);
    }
    
    protected virtual Node2D Game
    {
        get => _game.IsValid() ? _game : throw new NullReferenceException("Game is not valid");
        set => _game = value;
    }

    protected internal virtual void Setup(Node2D game)
    {
        Game = game;
    }
    
    /// <summary>
	/// Plays music at the specified path.
	/// </summary>
	/// <param name="path">Path to the music resource.</param>
	public virtual AudioStreamPlayer PlayMusic(string path)
	{
		if (CurrentMusic.IsValid())
		{
			CurrentMusic.QueueFree();
		}
		var stream = new AudioStreamPlayer();
		stream.Stream = GD.Load<AudioStream>(path);
		stream.Bus = MusicBus;
		stream.Autoplay = true;

		CurrentMusic = stream;
		Game.AddChild(stream);
		return stream;
	}

	/// <summary>
	/// Plays a UI sound at the specified path with optional volume.
	/// </summary>
	/// <param name="path">Path to the sound resource.</param>
	/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
	public virtual AudioStreamPlayer PlayUiSound(string path, float volume = 1)
	{
		var res = GD.Load<AudioStream>(path);
		var stream = new AudioStreamPlayer();
		stream.Stream = res;
		stream.Bus = SoundsBus;
		stream.VolumeDb = Mathf.LinearToDb(volume);
		_uiSounds.Add(stream);
		stream.Finished += () => 
		{ 
			stream.QueueFree();
			_uiSounds.Remove(stream);
		};
		stream.TreeExited += () =>
		{
			_uiSounds.Remove(stream);
		};
		stream.Autoplay = true;

		Game.AddChild(stream);
		return stream;
	}

	/// <summary>
	/// Plays a sound at the specified position in the game world with optional volume.
	/// </summary>
	/// <param name="path">Path to the sound resource.</param>
	/// <param name="position">Position in the game world.</param>
	/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
	public virtual AudioStreamPlayer2D PlaySoundAt(string path, Vector2 position, float volume = 1)
	{
		var stream = ConfigureSound(path,volume);

		Game.AddChild(stream);
		stream.GlobalPosition = position;
		stream.Autoplay = true;
		return stream;
	}

	/// <summary>
	/// Plays a sound attached to the specified node with optional volume.
	/// </summary>
	/// <param name="path">Path to the sound resource.</param>
	/// <param name="node">Node2D to attach the sound to.</param>
	/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
	public virtual AudioStreamPlayer2D PlaySoundOn(string path, Node2D node, float volume = 1)
	{
		var stream = ConfigureSound(path, volume);
		stream.Autoplay = true;

		node.AddChild(stream);
		return stream;
	}


	/// <summary>
	/// Clears all currently playing UI and world sounds.
	/// </summary>
	/// <remarks>
	/// This method stops and removes all UI and world sounds that are currently playing, 
	/// both from the UI sound pool and the world sound pool. 
	/// After calling this method, no UI or world sounds will be audible.
	/// </remarks>
	public virtual void ClearAllSounds()
	{
		foreach (var stream in _uiSounds)
			stream.QueueFree();

		foreach (var stream in _worldSounds)
			stream.QueueFree();
	}

	/// <summary>
	/// Configures a 2D sound with the specified path and volume. The sound will be removed after finishing.
	/// </summary>
	/// <param name="path">Path to the sound resource.</param>
	/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
	/// <returns>The configured AudioStreamPlayer2D instance.</returns>
	public virtual AudioStreamPlayer2D ConfigureSound(string path, float volume = 1)
	{
		var res = GD.Load<AudioStream>(path);
		var stream = new AudioStreamPlayer2D();
		stream.Stream = res;
		stream.Bus = SoundsBus;
		stream.VolumeDb = Mathf.LinearToDb(volume);
		_worldSounds.Add(stream);
		stream.Finished += () =>
		{
			stream.QueueFree();
			_worldSounds.Remove(stream);
		};
		stream.TreeExited += () =>
		{
			_worldSounds.Remove(stream);
		};

		return stream;
	}
    
    public override void Run()
    {
        Setup(References.Instance.WorldContainer);
    }
}