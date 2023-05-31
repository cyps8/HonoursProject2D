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

	Vector2 targetBlend = new Vector2(0, 0);

	bool walking = false;

	float backLegDelay = 0.525f;

	Character attachedCharacter;

    //Sprite2D sprite;

	public bool footGrounded = false;

	public bool GetFootGrounded()
	{
		return footGrounded;
	}

	public bool GetGrounded()
	{
		return grounded;
	}

	public void SetCharacter(Character _character)
	{
		attachedCharacter = _character;
	}

	public void AnimFootGrounded(bool _value)
	{
		footGrounded = _value;

		//GD.Print($"Foot grounded: {footGrounded}");
	}

    public AnimationPlayer GetAnimationPlayer()
	{
		return animationPlayer;
	}

	public void Walk(float _value)
	{
		if (GameManager.instance.GetMode() != Mode.PlayMode)
			return;

		if (!attached)
		{
			return;
		}

		if (!grounded)
		{
			return;
		}

		Vector2 moveVelocity;

		if (Mathf.Abs(_value) > 0.1f)
		{
			PlayWalk();
			moveVelocity = normal.Rotated(Mathf.DegToRad(90));
		}
		else
		{
			StopWalk();
			moveVelocity = normal.Rotated(Mathf.DegToRad(90));
		}

		//GD.Print($"Move Velocity: {moveVelocity}");

		if (footGrounded)
		{
			ApplyCentralImpulse(moveVelocity.Normalized() * 100 * -_value);

			if (grounded && LinearVelocity.Length() > 400)
			{
				LinearVelocity = LinearVelocity.Normalized() * 400;

				//GD.Print("Damped");
			}
		}
	}

	void PlayWalk()
	{
		if (backLeg)
		{
			if (backLegDelay > 0)
			{
				return;
			}
		}


		//animationTree.Set("parameters/blend_position", normal);
		targetBlend = new Vector2(normal.X, normal.Y * -1);

		walking = true;

		// if (!animationPlayer.IsPlaying())
		// {
		// 	GD.Print("Detected not playing");
		// 	animationPlayer.Play();
		// }

		//GD.Print($"Normal: {normal}");
	}

	void StopWalk()
	{
		if (backLeg)
		{
			backLegDelay = 0.525f;
		}


		//animationTree.Set("parameters/blend_position", new Vector2(0, 0));
		targetBlend = normal;

		walking = false;
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
	public override void _PhysicsProcess(double delta)
	{
		if (GameManager.instance.GetMode() != Mode.PlayMode)
			return;

		if (!attached)
		{
			return;
		}

		if (backLeg)
		{
			Modulate = new Color(0.5f, 0.5f, 0.5f);
		}

		if (GroundCheck())
		{ 
			grounded = true;
		}
		else
		{
			grounded = false;
			//animationTree.Set("parameters/blend_position", normal * 0.2f);
			targetBlend = new Vector2(normal.X, normal.Y * -1) * 0.2f;
		}

		if (backLeg)
		{
			backLegDelay -= (float)delta;
		}

		Vector2 oldBlend = (Vector2)animationTree.Get("parameters/blend_position");

		if (Mathf.Sign(oldBlend.Y) != Mathf.Sign(targetBlend.Y))
		{
			animationTree.Set("parameters/blend_position", targetBlend);
		}
		else
		{
			animationTree.Set("parameters/blend_position", new Vector2(Mathf.Lerp(oldBlend.X, targetBlend.X, 0.2f), Mathf.Lerp(oldBlend.Y, targetBlend.Y, 0.1f)));
		}
	}

	int GetFootNum()
	{
		int footNum = 0;
		foreach (var leg in attachedCharacter.legs)
		{
			footNum++;
			if (leg == this)
			{
				return footNum;
			}
		}
		return -1;
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

			//normal = (floorAngle.Rotated(Mathf.DegToRad(90))).Normalized();

			contactPoint = contacts[0] + (floorAngle / 2);

			normal = (contactPoint - GlobalPosition).Normalized();
        }

		return onGround;
    }
}
