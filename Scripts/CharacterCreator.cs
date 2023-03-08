using Godot;
using System;
using Godot.Collections;

public partial class CharacterCreator : Node2D
{
    public static CharacterCreator instance;

    [Export] PackedScene LegIns;

	bool placeLeg = false;

	Character selectedCharacterRef;

	CanvasLayer hud;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (instance == null)
        {
            instance = this;
        }

        hud = GetNode<CanvasLayer>("HUD");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Reset()
	{
		hud.Visible = true;
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
                RigidBody2D part = selectedCharacterRef;

				if (selectedCharacterRef.legs.Count > 1)
				{
					part = selectedCharacterRef.part1;
				}
                if (selectedCharacterRef.legs.Count > 3)
                {
                    part = selectedCharacterRef.part2;
                }
                if (selectedCharacterRef.legs.Count > 5)
                {
                    part = selectedCharacterRef.part3;
                }

                placeLeg = false;

				{

                    var leg = (Leg)LegIns.Instantiate();
                    leg.Position = GetGlobalMousePosition() - selectedCharacterRef.Position;
                    part.AddChild(leg);
                    selectedCharacterRef.legs.Add(leg);

                    PinJoint2D joint = new PinJoint2D();
                    leg.AddChild(joint);
                    joint.NodeA = part.GetPath();
                    joint.NodeB = leg.GetPath();
                }
                {
                    var leg = (Leg)LegIns.Instantiate();
                    leg.Position = GetGlobalMousePosition() - selectedCharacterRef.Position;
                    part.AddChild(leg);
                    selectedCharacterRef.legs.Add(leg);

                    PinJoint2D joint = new PinJoint2D();
                    leg.AddChild(joint);
                    joint.NodeA = part.GetPath();
                    joint.NodeB = leg.GetPath();

					leg.SetBackLeg();
                }
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
