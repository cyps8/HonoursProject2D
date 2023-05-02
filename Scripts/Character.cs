using Godot;
using System;
using Godot.Collections;

public enum PlayerState { Idle, Walking }

public partial class Character : RigidBody2D
{
	public Array<Leg> legs = new Array<Leg>();

	public int groundedLegs = 0;

	public PlayerState currentState;

	float walkTimer;

	public Array<RigidBody2D> bodyParts = new Array<RigidBody2D>();

	public Array<Joint2D> pinJoints = new Array<Joint2D>();

	public Array<Body> decorParts = new Array<Body>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentState = PlayerState.Idle;

		walkTimer = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// foreach (Body part in decorParts)
		// {
		// 	part.Rotation = ((RigidBody2D)part.GetParent()).Rotation;
		// }
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GameManager.instance.GetMode() != Mode.PlayMode)
			return;

		//var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("ui_right"))
		{
			foreach (var leg in legs)
			{
				leg.Walk(1f);
			}
			//velocity.X = 1;
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			foreach (var leg in legs)
			{
				leg.Walk(-1f);
			}
			//velocity.X -= 1;
		}
		else
		{
			foreach (var leg in legs)
			{
				leg.Walk(0f);
			}
		}
	}

	public void Play()
	{
		Freeze = false;

		foreach (RigidBody2D bodyPart in bodyParts)
		{
			bodyPart.Freeze = false;
		}

        foreach (var leg in legs)
		{
			leg.Freeze = false;
		}
	}

	public void Reset()
	{
		Freeze = true;

        foreach (RigidBody2D bodyPart in bodyParts)
        {
            bodyPart.Freeze = true;
        }

        foreach (var leg in legs)
		{
			leg.Freeze = true;
		}

		//foreach (var leg in legs)
		//{
		//	RemoveChild(leg);
		//}

		//legs.Clear();
	}

	public void ResetPose()
	{
        Freeze = true;

		Rotation = 0;

        foreach (Body bodyPart in bodyParts)
        {
            bodyPart.Freeze = true;
			bodyPart.Rotation = 0;
			bodyPart.ReturnToOrigin();
        }

        foreach (var leg in legs)
        {
            leg.Freeze = true;
			leg.Rotation = 0;
        }
    }
}
