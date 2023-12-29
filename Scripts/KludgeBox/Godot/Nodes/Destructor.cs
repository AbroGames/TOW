﻿using Godot;
using TOW.Scripts.KludgeBox.Scheduling;


namespace KludgeBox.Godot.Nodes;

public partial class Destructor : Node
{
    public bool IsPaused { get; set; } = false;

    public double TimeLeft
    {
        get => _cooldown.TimeLeft;
        set
        {
            _cooldown.Duration = value;
            _cooldown.Restart();
        }
    }

    private Cooldown _cooldown = new Cooldown();

    public Destructor()
    {
        _cooldown.OnReady += Destruct;
    }

    public Destructor(double time)
    {
        TimeLeft = time;
        _cooldown.OnReady += Destruct;
    }

    public override void _Process(double delta)
    {
        _cooldown.Update(delta);
    }

    private void Destruct()
    {
        var parent = GetParent();
        if (IsInstanceValid(parent))
            parent.QueueFree();
    }
}