using System;
using System.Collections.Generic;
using Godot;
using TOW.Scripts.KludgeBox.Collections;
using TOW.Scripts.KludgeBox.Core;
using TOW.Scripts.KludgeBox.Godot.Extensions;

namespace KludgeBox.Godot
{
	/// <summary>
	/// Static class for managing audio in the game.
	/// </summary>
	public static class Audio2D
	{
		public static bool Ready { get; private set; } = false;
		
		/// <summary>
		/// Name of the master bus.
		/// </summary>
		public static readonly string MasterBus = "Master";

		/// <summary>
		/// Name of the music bus.
		/// </summary>
		public static readonly string MusicBus = "Music";

		/// <summary>
		/// Name of the sounds bus.
		/// </summary>
		public static readonly string SoundsBus = "Sounds";

		/// <summary>
		/// Index of the master bus.
		/// </summary>
		public static int MasterIndex => AudioServer.GetBusIndex(MasterBus);

		/// <summary>
		/// Index of the music bus.
		/// </summary>
		public static int MusicIndex => AudioServer.GetBusIndex(MusicBus);

		/// <summary>
		/// Index of the sounds bus.
		/// </summary>
		public static int SoundsIndex => AudioServer.GetBusIndex(SoundsBus);

		/// <summary>
		/// The currently playing music stream player.
		/// </summary>
		public static AudioStreamPlayer CurrentMusic { get; private set; } = null;

		/// <summary>
		/// Read-only collection of currently playing UI sounds.
		/// </summary>
		public static ReadOnlyHashSet<AudioStreamPlayer> PlayingUiSounds => _uiSounds.AsReadOnly();

		/// <summary>
		/// Read-only collection of currently playing world sounds.
		/// </summary>
		public static ReadOnlyHashSet<AudioStreamPlayer2D> PlayingWorldSounds => _worldSounds.AsReadOnly();

		private static HashSet<AudioStreamPlayer> _uiSounds = new HashSet<AudioStreamPlayer>();
		private static HashSet<AudioStreamPlayer2D> _worldSounds = new HashSet<AudioStreamPlayer2D>();
		private static Node2D _world;
		private static Node2D _main;


		/// <summary>
		/// Master volume in linear scale (0.0 to 1.0).
		/// </summary>
		public static float MasterVolume
		{
			get => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(MasterIndex));
			set => AudioServer.SetBusVolumeDb(MasterIndex, Mathf.LinearToDb(value));
		}

		/// <summary>
		/// Master volume in decibels.
		/// </summary>
		public static float MasterVolumeDb
		{
			get => AudioServer.GetBusVolumeDb(MasterIndex);
			set => AudioServer.SetBusVolumeDb(MasterIndex, value);
		}

		/// <summary>
		/// Music volume in linear scale (0.0 to 1.0).
		/// </summary>
		public static float MusicVolume
		{
			get => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(MusicIndex));
			set => AudioServer.SetBusVolumeDb(MusicIndex, Mathf.LinearToDb(value));
		}

		/// <summary>
		/// Music volume in decibels.
		/// </summary>
		public static float MusicVolumeDb
		{
			get => AudioServer.GetBusVolumeDb(MusicIndex);
			set => AudioServer.SetBusVolumeDb(MusicIndex, value);
		}

		/// <summary>
		/// Sounds volume in linear scale (0.0 to 1.0).
		/// </summary>
		public static float SoundsVolume
		{
			get => Mathf.DbToLinear(AudioServer.GetBusVolumeDb(SoundsIndex));
			set => AudioServer.SetBusVolumeDb(SoundsIndex, Mathf.LinearToDb(value));
		}

		/// <summary>
		/// Sounds volume in decibels.
		/// </summary>
		public static float SoundsVolumeDb
		{
			get => AudioServer.GetBusVolumeDb(SoundsIndex);
			set => AudioServer.SetBusVolumeDb(SoundsIndex, value);
		}


		private static void DoReadyCheck()
		{
			if (!Ready)
				throw new InvalidOperationException("Audio2D is not ready");
		}

		public static Node2D Main
		{
			get => _main.IsValid() ? _main : throw new NullReferenceException("Main is not valid");
			private set => _main = value;
		}

		public static Node2D World
		{
			get => _world.IsValid() ? _world : throw new NullReferenceException("World is not valid");
			private set => _world = value;
		}

		public static void Setup(Node2D main, Node2D world)
		{
			Main = main;
			World = world;
			Ready = true;
		}
		
		/// <summary>
		/// Plays music at the specified path.
		/// </summary>
		/// <param name="path">Path to the music resource.</param>
		public static AudioStreamPlayer PlayMusic(string path)
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
			Main.AddChild(stream);
			return stream;
		}

		/// <summary>
		/// Plays a UI sound at the specified path with optional volume.
		/// </summary>
		/// <param name="path">Path to the sound resource.</param>
		/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
		public static AudioStreamPlayer PlayUiSound(string path, float volume = 1)
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

			Main.AddChild(stream);
			return stream;
		}

		/// <summary>
		/// Plays a sound at the specified position in the game world with optional volume.
		/// </summary>
		/// <param name="path">Path to the sound resource.</param>
		/// <param name="position">Position in the game world.</param>
		/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
		public static AudioStreamPlayer2D PlaySoundAt(string path, Vector2 position, float volume = 1)
		{
			var stream = ConfigureSound(path,volume);

			World.AddChild(stream);
			stream.Position = position;
			stream.Autoplay = true;
			return stream;
		}

		/// <summary>
		/// Plays a random sound from the provided paths at the specified position in the game world with optional volume.
		/// </summary>
		/// <param name="paths">List of paths to the sound resources.</param>
		/// <param name="position">Position in the game world.</param>
		/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
		public static AudioStreamPlayer2D PlaySoundAt(IEnumerable<string> paths, Vector2 position, float volume = 1)
		{
			return PlaySoundAt(paths.GetRandom(), position, volume);
		}

		/// <summary>
		/// Plays a sound attached to the specified node with optional volume.
		/// </summary>
		/// <param name="path">Path to the sound resource.</param>
		/// <param name="node">Node2D to attach the sound to.</param>
		/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
		public static AudioStreamPlayer2D PlaySoundOn(string path, Node2D node, float volume = 1)
		{
			var stream = ConfigureSound(path, volume);
			stream.Autoplay = true;

			node.AddChild(stream);
			return stream;
		}

		/// <summary>
		/// Plays a random sound from the provided paths attached to the specified node with optional volume.
		/// </summary>
		/// <param name="paths">List of paths to the sound resources.</param>
		/// <param name="node">Node2D to attach the sound to.</param>
		/// <param name="volume">Volume of the sound (0.0 to 1.0).</param>
		public static AudioStreamPlayer2D PlaySoundOn(IEnumerable<string> paths, Node2D node, float volume = 1)
		{
			return PlaySoundOn(paths.GetRandom(), node, volume);
		}

		/// <summary>
		/// Clears all currently playing UI and world sounds.
		/// </summary>
		/// <remarks>
		/// This method stops and removes all UI and world sounds that are currently playing, 
		/// both from the UI sound pool and the world sound pool. 
		/// After calling this method, no UI or world sounds will be audible.
		/// </remarks>
		public static void ClearAllSounds()
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
		public static AudioStreamPlayer2D ConfigureSound(string path, float volume = 1)
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
	}
}
