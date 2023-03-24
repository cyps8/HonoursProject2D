using Godot;
using System;

public partial class Body : RigidBody2D
{
	Vector2 origin;

	public void SetOrigin()
	{
		origin = Position;
	}

	public void ReturnToOrigin()
	{
		Position = origin;
	}
}
