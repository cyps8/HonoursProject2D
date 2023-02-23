using Godot;
using System;
using Godot.Collections;

public partial class CharacterCreator : Node2D
{
	[Export] PackedScene LegIns;

	bool placeLeg = false;

	Character selectedCharacterRef;

	CanvasLayer hud;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hud = GetNode<CanvasLayer>("HUD");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	public void BPAddLeg() // Button pressed "Add leg"
	{
		placeLeg = true;
	}

	public void BPScreen() // Button pressed "Screen"
	{
		selectedCharacterRef = GameManager.instance.playerCharacter;

		var space = GetWorld2D().DirectSpaceState;

		PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();

		query.Position = GetGlobalMousePosition();

		query.CollisionMask = 8;

		var result = space.IntersectPoint(query);

		if (result.Count > 0)
		{
			if (placeLeg)
			{
				placeLeg = false;
				var leg = (Leg)LegIns.Instantiate();
				leg.Position = GetGlobalMousePosition() - selectedCharacterRef.Position;
				selectedCharacterRef.AddChild(leg);
				selectedCharacterRef.legs.Add(leg);

				PinJoint2D joint = new PinJoint2D();
				leg.AddChild(joint);
				joint.NodeA = selectedCharacterRef.GetPath();
				joint.NodeB = leg.GetPath();
			}
		}
	}

	public void BPPlay()
	{
		selectedCharacterRef.Play();

		hud.Visible = false;

		GameManager.instance.SetMode(Mode.PlayMode);
	}
}
