using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TOW.Scripts.KludgeBox.Collections;

public partial class Services : Node
{
	public static Services Instance { get; private set; }

	private readonly Dictionary<Type, Node> _services = new();
	public ReadOnlyDictionary<Type, Node> RegisteredServices => _services.AsReadOnly();
	
	public override void _Ready()
	{
		Instance = this;
		var children = GetChildren();
		
		foreach (var child in children)
		{
			Set(child);
		}
	}

	public static void Set<T>(T service) where T : Node
	{
		if(service is null) return;
		
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Node existingService))
		{
			existingService.QueueFree();
		}
		
		Instance.AddChild(service);
		Instance._services[type] = service;
	}

	public static T Get<T>() where T : Node
	{
		var type = typeof(T);
		return Instance._services[type] as T;
	}

	public static T GetOrDefault<T>(T service) where T : Node
	{
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Node existingService))
		{
			return existingService as T;
		}
		
		Set(service);
		return service;
	}
	
	public static T ForceGet<T>() where T : Node, new()
	{
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Node service))
		{
			return service as T;
		}

		var newService = new T();
		Set(newService);
		return newService;
	}
}
