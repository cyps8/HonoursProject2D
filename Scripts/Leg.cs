using Godot;
using System;
using System.Diagnostics;

public partial class Leg : RigidBody2D
{
	AnimationPlayer animationPlayer;

	Sprite2D sprite;

	public AnimationPlayer GetAnimationPlayer()
	{
		return animationPlayer;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        var space = GetWorld2D().DirectSpaceState;

        PhysicsRayQueryParameters2D query = new PhysicsRayQueryParameters2D();

		query.From = sprite.Position;

		query.To = sprite.Position + (Vector2.Down * 500);

		var result = space.IntersectRay(query);

		if (result.Count > 0)
		{
			GD.Print("ooooo");
		}
	}
}
