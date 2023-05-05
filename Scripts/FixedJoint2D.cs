using Godot;
using System;

public partial class FixedJoint2D : PinJoint2D
{
	float rotationFix = 0;

	public void ConnectNodes(PhysicsBody2D _nodeA, PhysicsBody2D _nodeB)
	{
		NodeA = _nodeA.GetPath();
		NodeB = _nodeB.GetPath();

		float angleToBody = (GlobalPosition - _nodeB.GlobalPosition).Angle();
		rotationFix = ((PhysicsBody2D)GetParent()).GlobalRotation - angleToBody;

		_nodeA.Connect("tree_exiting", Callable.From(this.DisconnectNodes));
		_nodeB.Connect("tree_exiting", Callable.From(this.DisconnectNodes));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (NodeA == null || NodeB == null)
			return;

		PhysicsBody2D bodyB = (PhysicsBody2D)GetNode(NodeB);

		float angleToBody = (GlobalPosition - bodyB.GlobalPosition).Angle();
		((PhysicsBody2D)GetParent()).SetDeferred("rotation", angleToBody + rotationFix);
	}

	void DisconnectNodes()
	{
		NodeA = null;
		NodeB = null;
	}
}
