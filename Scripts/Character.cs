using Godot;
using System;
using System.Collections.Generic;

public partial class Character : RigidBody2D
{
	public List<RigidBody2D> legs = new List<RigidBody2D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play()
	{
		Freeze = false;

		foreach (var leg in legs)
		{
			leg.Freeze = false;
		}
	}
}
