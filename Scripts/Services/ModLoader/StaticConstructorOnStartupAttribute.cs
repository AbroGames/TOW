using System;

namespace TOW.Scripts.Services.ModLoader;

[AttributeUsage(AttributeTargets.Class)]
public class StaticConstructorOnStartupAttribute : Attribute{}