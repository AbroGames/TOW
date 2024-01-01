using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Godot;
using TOW.Scripts.KludgeBox.VFS.Base;
using TOW.Scripts.KludgeBox.VFS.FileSystems;
using TOW.Scripts.KludgeBox.Core;
using TOW.Scripts.Utils;

namespace TOW.Scripts.Services.ModLoader;


[GlobalClass]
public partial class ModLoader : Services.Service
{
    private FileSystem _userModsFs = new GodotFileSystem(GodotFsRoot.User,null, true);
    private List<Mod> _mods = [];
    private FsDirectory _modsDir;
    private List<Assembly> _loadedAssemblies = [];
    
    public virtual ReadOnlyCollection<Assembly> LoadedAssemblies => _loadedAssemblies.AsReadOnly();
    
    

    private List<Assembly> Load(IEnumerable<FsFile> dlls)
    {
        var loadedAssemblies = new List<Assembly>();
        var alc = GetCurrentAlc();
        foreach (var dll in dlls)
        {
            try
            {
                var assembly = alc.LoadFromStream(dll.OpenRead());
                //var assembly = Assembly.Load(dll.ReadAllBytes());
                loadedAssemblies.Add(assembly);
            }
            catch (Exception e)
            {
                Log.Error($"Error loading assembly from {dll.Path}: {e.Message}");
            }
        }

        return loadedAssemblies;
    }

    private AssemblyLoadContext GetCurrentAlc()
    {
        // Get the currently executing assembly
        Assembly asm = Assembly.GetExecutingAssembly();

        // Get the AssemblyLoadContext for the currently executing assembly
        AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(asm);

        return loadContext;
    }
    
    private Mod Run(Assembly assembly)
    {
        var alcs = AssemblyLoadContext.All;
        Log.Debug($"Following {alcs.Count()} ALCs are presented:");
        foreach (var alc in alcs)
        {
            Log.Debug($"'{alc.Name}' contains assemblies:");
            foreach (var alcAssembly in alc.Assemblies)
            {
                Log.Debug($"\t{alcAssembly}");
            }
        }

        Log.Info();
        
        var mods = assembly.FindAllTypesThatDeriveFrom<Mod>();
        if (mods.Count() > 1) throw new TooMuchModsException();

        Mod mod = null;
        
        if (mods.Any())
        {
            try
            {
                mod = ReflectionExtensions.GetInstanceOfType(mods.First()) as Mod;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        var scripts = assembly.FindTypesWithAttributes(typeof(StaticConstructorOnStartupAttribute));
        foreach (var script in scripts)
        {
            script.TypeInitializer?.Invoke(null, null);
        }
        
        if(mod is not null) AddChild(mod);
        
        return mod;
    }

    public override void Run()
    {
        _modsDir = _userModsFs.GetDirectory("/Mods");
        var dlls = _modsDir.Files.Where(file => file.Path.EndsWith(".dll"));
        var assemblies = Load(dlls);
        foreach (var assembly in assemblies)
        {
            try
            {
                _mods.Add(Run(assembly));
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }
    }
}