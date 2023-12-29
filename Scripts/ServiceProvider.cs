using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Godot;
using TOW.Scripts.KludgeBox.Collections;
using TOW.Scripts.Services;
using TOW.Scripts.Services.ModLoader;

namespace TOW.Scripts;

/// <summary>
/// Represents a class that manages services as nodes.
/// </summary>
public partial class ServiceProvider : Node
{
	/// <summary>
	/// Gets the singleton instance of the Services class.
	/// </summary>
	public static ServiceProvider Instance { get; private set; }

	private readonly Dictionary<Type, Service> _services = new();
	
	/// <summary>
	/// Gets a read-only dictionary of registered services, where the key is the service type and the value is the corresponding Node.
	/// </summary>
	public ReadOnlyDictionary<Type, Service> RegisteredServices => _services.AsReadOnly();
	
	public override void _Ready()
	{
		Instance = this;
		var children = GetChildren();
		
		foreach (var child in children)
		{
			if (child is Service service)
			{
				Register(service);
			}
		}
		
		foreach (var child in children)
		{
			if (child is Service service)
			{
				service.Run();
			}
		}
	}

	/// <summary>
	/// Sets a service of the specified type, replacing any existing service of the same type.
	/// </summary>
	/// <typeparam name="T">The type of the service, must be a subclass of Node.</typeparam>
	/// <param name="service">The service instance to be set.</param>
	public static void Set<T>(T service) where T : Service
	{
		if(service is null) return;
		
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Service existingService))
		{
			existingService.QueueFree();
		}
		
		Instance.AddChild(service);
		Instance._services[type] = service;
	}

	private static void Register<T>(T service) where T : Service
	{
		if(service is null) return;
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Service existingService))
		{
			existingService.QueueFree();
		}
		
		Instance._services[type] = service;
	}

	/// <summary>
	/// Gets the service of the specified type, if registered.
	/// </summary>
	/// <typeparam name="T">The type of the service, must be a subclass of Node.</typeparam>
	/// <returns>The service instance if registered, otherwise null.</returns>
	public static T Get<T>() where T : Service
	{
		var type = typeof(T);
		return Instance._services[type] as T;
	}

	/// <summary>
	/// Gets the registered service of the specified type, or sets and returns the provided service if not registered.
	/// </summary>
	/// <typeparam name="T">The type of the service, must be a subclass of Node.</typeparam>
	/// <param name="service">The service instance to be returned if not registered.</param>
	/// <returns>The registered service or the provided service if not registered.</returns>
	public static T GetOrDefault<T>(T service) where T : Service
	{
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Service existingService))
		{
			return existingService as T;
		}
		
		Set(service);
		return service;
	}
	
	/// <summary>
	/// Gets the registered service of the specified type, or creates, sets, and returns a new instance if not registered.
	/// </summary>
	/// <typeparam name="T">The type of the service, must be a subclass of Node and must have a parameterless constructor.</typeparam>
	/// <returns>The registered service or a newly created and registered service instance.</returns>
	public static T ForceGet<T>() where T : Service, new()
	{
		var type = typeof(T);
		if (Instance._services.TryGetValue(type, out Service service))
		{
			return service as T;
		}

		var newService = new T();
		Set(newService);
		return newService;
	}
	
}