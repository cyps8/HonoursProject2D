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

	Array<float> placeRadius = new Array<float>();

	RigidBody2D ghostPart;

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

		placeRadius.Add(10f);

		placeRadius.Add(40f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.instance.GetMode() == Mode.EditMode)
		{
			Ghost();
		}
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

	void Ghost()
	{
        if (placePart)
        {
            switch (placePartId)
            {
                case 0:
                    if (ghostPart == null)
                    {
                        ghostPart = (RigidBody2D)LegIns.Instantiate();
                        AddChild(ghostPart);
                        ghostPart.ZIndex = 10;
                        ghostPart.CollisionLayer = 0;
                    }
                    break;
                case 1:
                    if (ghostPart == null)
                    {
                        ghostPart = (RigidBody2D)BodyIns.Instantiate();
                        AddChild(ghostPart);
						ghostPart.ZIndex = 10;
						ghostPart.CollisionLayer = 0;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (ghostPart != null)
            {
                ghostPart.Visible = false;
                ghostPart = null;
            }
        }

        if (ghostPart == null)
			return;

		ghostPart.GlobalPosition = GetGlobalMousePosition();

        var space = GetWorld2D().DirectSpaceState;

        var query = new PhysicsShapeQueryParameters2D();

        CircleShape2D shape = new CircleShape2D();

        shape.Radius = placeRadius[placePartId];

        query.Shape = shape;

        query.CollisionMask = 8;

        query.Transform = new Transform2D(0, GetGlobalMousePosition());

        var result = space.IntersectShape(query);

		if (result.Count > 0)
		{
			ghostPart.Modulate = new Color(0.4f, 1f, 0.4f);
		}
		else
		{
            ghostPart.Modulate = new Color(1f, 0.4f, 0.4f);
        }	
    }


	public void BPAddPart(int _partId) // Button pressed "Add leg"
	{
		if (placePart && placePartId == _partId)
		{
			placePart = false;
			return;
		}

		placePart = true;

		placePartId = _partId;
	}

	public void BPScreen() // Button pressed "Screen"
	{
		selectedCharacterRef = GameManager.instance.playerCharacter;

		var space = GetWorld2D().DirectSpaceState;

        var query = new PhysicsShapeQueryParameters2D();

        CircleShape2D shape = new CircleShape2D();

        shape.Radius = placeRadius[placePartId];

        query.Shape = shape;

		query.CollisionMask = 8;

        query.Transform = new Transform2D(0, GetGlobalMousePosition());

        var result = space.IntersectShape(query);

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

		joint.Position = (_part.GlobalPosition - body.GlobalPosition) / 2;
		joint.DisableCollision = false;
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
