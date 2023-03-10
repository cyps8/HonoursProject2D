using Godot;
using System;

public partial class Leg : RigidBody2D
{
	AnimationPlayer animationPlayer;

	bool BackLeg = false;

	bool grounded = false;

	//Sprite2D sprite;

	public AnimationPlayer GetAnimationPlayer()
	{
		return animationPlayer;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		//sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public void SetBackLeg()
	{
		Modulate = new Color(0.5f, 0.5f, 0.5f);

		ZIndex = -1;

		BackLeg = true;
	}


	public bool GetBackLeg()
	{
		return BackLeg;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
				if (((PhysicsBody2D)hit["collider"]).IsInGroup("Ground"))
					return true;
			}
		}

		return false;
	}
}
