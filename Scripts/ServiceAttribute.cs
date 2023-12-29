using System;

namespace TOW.Scripts;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute : Attribute
{
    public Type ServiceType { get; private set; } = null;

    public ServiceAttribute() {}

    public ServiceAttribute(Type serviceType)
    {
        ServiceType = serviceType;
    }
}