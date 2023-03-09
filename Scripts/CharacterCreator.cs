using Godot;
using System;
using Godot.Collections;

public partial class CharacterCreator : Node2D
{
    public static CharacterCreator instance;

    [Export] PackedScene LegIns;

	[Export] PackedScene BodyIns;

	bool placePart = false;

	int placePartId = 0;

	Character selectedCharacterRef;

	CanvasLayer hud;

	[Export] GridContainer buttonContainer;

	Array<ButtonData> buttons = new Array<ButtonData>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        if (instance == null)
        {
            instance = this;
        }

		int i = buttonContainer.GetChildCount();

		for (int j = 0; j < i; j++)
		{
			buttons.Add((ButtonData)buttonContainer.GetChild(j));
		}

        hud = GetNode<CanvasLayer>("HUD");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void TabChanged(int _tabId)
	{
		switch (_tabId)
		{
			case 0:
				foreach (ButtonData button in buttons)
				{
					button.Visible = true;
				}
				break;
            case 1:
                foreach (ButtonData button in buttons)
                {
                    if (button.GetPartData() == PartData.Leg)
					{
						button.Visible = true;
					}
					else
					{
						button.Visible = false;
					}
                }
                break;
            case 2:
                foreach (ButtonData button in buttons)
                {
                    if (button.GetPartData() == PartData.Body)
                    {
                        button.Visible = true;
                    }
                    else
                    {
                        button.Visible = false;
                    }
                }
                break;
        }
	}

	public void Reset()
	{
		hud.Visible = true;
	}


	public void BPAddPart(int _partId) // Button pressed "Add leg"
	{
		placePart = true;

		placePartId = _partId;
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
			if (placePart)
			{
                RigidBody2D part = (RigidBody2D)result[0]["collider"];

				placePart = false;

				if (placePartId == 0)
				{
					PlaceLeg(part, false);
					PlaceLeg(part, true);
				}

				if (placePartId == 1)
				{
					PlaceBody(part);
				}
            }
		}
	}

	void PlaceBody(RigidBody2D _part)
	{
        var body = (Body)BodyIns.Instantiate();
        body.Position = GetGlobalMousePosition() - _part.GlobalPosition;
        _part.AddChild(body);
        selectedCharacterRef.bodyParts.Add(body);

        PinJoint2D joint = new PinJoint2D();
        body.AddChild(joint);
        joint.NodeA = _part.GetPath();
        joint.NodeB = body.GetPath();

		joint.Position = _part.GlobalPosition - body.GlobalPosition;
    }

	void PlaceLeg(RigidBody2D _part, bool _backLeg)
	{
        var leg = (Leg)LegIns.Instantiate();
        leg.Position = GetGlobalMousePosition() - _part.GlobalPosition;
        _part.AddChild(leg);
        selectedCharacterRef.legs.Add(leg);

        PinJoint2D joint = new PinJoint2D();
        leg.AddChild(joint);
        joint.NodeA = _part.GetPath();
        joint.NodeB = leg.GetPath();

		if (_backLeg)
		{
			leg.SetBackLeg();
		}
    }

	public void BPPlay()
	{
		selectedCharacterRef.Play();

		hud.Visible = false;

		GameManager.instance.SetMode(Mode.PlayMode);
	}
}
