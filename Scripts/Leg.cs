using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

public partial class Leg : RigidBody2D
{
	AnimationPlayer animationPlayer;

	AnimationTree animationTree;

	bool backLeg = false;

	bool grounded = false;

	public bool attached = false;

	Vector2 normal = new Vector2(0, 1);

	Vector2 contactPoint = new Vector2(0, 0);

    //Sprite2D sprite;

    public AnimationPlayer GetAnimationPlayer()
	{
		return animationPlayer;
	}

	public void PlayWalk()
	{
		animationTree.Set("parameters/blend_position", (-normal * 1.5f));

		GD.Print($"Normal: {(-normal * 1.5f)}");
	}

	public void StopWalk()
	{
		animationTree.Set("parameters/blend_position", new Vector2(0, 0));
	}

	public AnimationTree GetAnimationTree()
	{
		return animationTree;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		animationTree = GetNode<AnimationTree>("AnimationTree");

		animationTree.Active = true;

		animationTree.Set("parameters/blend_position", new Vector2(0, 0));

		//sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public void SetBackLeg()
	{
		Modulate = new Color(0.5f, 0.5f, 0.5f);

		ZIndex = -1;

		backLeg = true;
	}


	public bool GetBackLeg()
	{
		return backLeg;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!attached)
		{
			return;
		}

		if (GroundCheck())
		{ 
			grounded = true;
		}
		else
		{
			grounded = false;
			animationPlayer.Play("Idle");
			animationPlayer.SpeedScale = 1;
		}
	}

	bool GroundCheck()
	{
		var space = GetWorld2D().DirectSpaceState;

		var query = new PhysicsShapeQueryParameters2D();

		CircleShape2D shape = new CircleShape2D();

		shape.Radius = 100f;

		query.Shape = shape;

		query.Transform = new Transform2D(0, GlobalPosition);

		var result = space.IntersectShape(query);

		if (result.Count > 0)
		{
			foreach (var hit in result)
			{
                if ( ((PhysicsBody2D)hit["collider"]).IsInGroup("Ground"))
				{
					return FindNormal(shape, (PhysicsBody2D)hit["collider"]);
				}
			}
		}

		return false;
	}

	bool FindNormal(CircleShape2D shape, PhysicsBody2D collider)
	{
        bool onGround = true;

        CollisionShape2D collisionShape = (CollisionShape2D)collider.GetChild(0);
        var contacts = shape.CollideAndGetContacts(GlobalTransform, collisionShape.Shape, collisionShape.GetGlobalTransform());

        foreach (Vector2 point in contacts)
        {
            if (point.Y < GlobalPosition.Y)
            {
                onGround = false;
            }
        }

        if (onGround)
        {
			Vector2 floorAngle = (contacts[0] - contacts[1]);

			normal = (floorAngle.Rotated(Mathf.RadToDeg(90))).Normalized();

			contactPoint = contacts[0] + (floorAngle / 2);
        }

		return onGround;
    }
}
