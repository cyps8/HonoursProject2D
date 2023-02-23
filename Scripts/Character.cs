using Godot;
using System;
using System.Collections.Generic;

public partial class Character : RigidBody2D
{
	public List<Leg> legs = new List<Leg>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GameManager.instance.GetMode() != Mode.PlayMode)
            return;

        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("ui_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("ui_left"))
        {
            velocity.X -= 1;
        }

        if (velocity.Length() > 0)
        {
            LinearVelocity = velocity.Normalized() * 200;
            foreach (var leg in legs)
            {
                leg.GetAnimationPlayer().Play("Walk");
                leg.GetAnimationPlayer().SpeedScale = Mathf.Sign(LinearVelocity.X);
            }
        }
        else
        {
            foreach (var leg in legs)
            {
                leg.GetAnimationPlayer().Play("Idle");
            }
        }
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
