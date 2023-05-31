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

	public void Paint(Color color)
	{
        var children = GetChildren();

        foreach (var child in children)
        {
            if (child.IsInGroup("Paintable"))
            {
				((Sprite2D)child).Modulate = color;
            }
        }
    }
}
