using Godot;
using System;
using Godot.Collections;

public enum PlayerState { Idle, Walking }

public partial class Character : RigidBody2D
{
	public Array<Leg> legs = new Array<Leg>();

	public PlayerState currentState;

	float walkTimer;

	public Array<RigidBody2D> bodyParts = new Array<RigidBody2D>();

	public Array<Joint2D> pinJoints = new Array<Joint2D>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentState = PlayerState.Idle;

		walkTimer = 0;
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
			currentState = PlayerState.Walking;

			walkTimer += (float)delta;

			LinearVelocity = (velocity.Normalized() * 800) + new Vector2(0, LinearVelocity.Y);
			foreach (var leg in legs)
			{
				leg.PlayWalk();

				if (leg.GetBackLeg())
				{
					if (walkTimer < 0.25f)
					{
                        leg.GetAnimationPlayer().SpeedScale = 0;
                    }
                }
			}
		}
		else
		{
            currentState = PlayerState.Idle;

			walkTimer = 0f;

            foreach (var leg in legs)
			{
				leg.StopWalk();
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
