using Godot;
using System;
using Godot.Collections;

public partial class CharacterCreator : Node2D
{
	public static CharacterCreator instance;

	[Export] PackedScene LegIns;

	[Export] PackedScene BodyIns;

	[Export] PackedScene EyeIns;

	[Export] PackedScene BeakIns;

	bool placePart = false;

	int placePartId = 0;

	int ghostPartId = -1;

	Character selectedCharacterRef;

	CanvasLayer hud;

	[Export] GridContainer buttonContainer;

	Array<ButtonData> buttons = new Array<ButtonData>();

	Array<float> placeRadius = new Array<float>();

	RigidBody2D ghostPart;

	Array<Body> bodyPool = new Array<Body>();

	Array<Leg> legPool = new Array<Leg>();

	Array<Body> eyePool = new Array<Body>();

	Array<Body> beakPool = new Array<Body>();

	Vector2 placePos;

	int currentTool = 0;

	RigidBody2D selectedPart;

	[Export] Label toolText;

	bool screenPressed = false;

	Vector2 toolOffset = new Vector2(0, 0);

	Vector2 moveOriginalPos = new Vector2(0, 0);

	Color pickedColour = new Color(0,0,0,1);

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

		placeRadius.Add(5f);

		placeRadius.Add(5f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GameManager.instance.GetMode() == Mode.EditMode)
		{
			Ghost();
		}

		if (screenPressed && selectedPart != null)
		{
			if (currentTool == 1)
			{
				Vector2 mousePos = GetGlobalMousePosition();

				selectedPart.GlobalPosition = mousePos + toolOffset;
			}
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
			case 3:
				foreach (ButtonData button in buttons)
				{
					if (button.GetPartData() == PartData.Decor)
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
		selectedCharacterRef.Position = new Vector2(300, 300);

		hud.Visible = true;

		GD.Print("wub wub");
	}

	void Ghost()
	{
		if (placePart)
		{
			if (ghostPartId != placePartId)
			{
				if (ghostPartId != -1)
				{
					ghostPart.Visible = false;

					if (ghostPart.IsInGroup("Leg"))
					{
						legPool.Add((Leg)ghostPart);
					}
					else if (ghostPart.IsInGroup("Body"))
					{
						bodyPool.Add((Body)ghostPart);
					}
					else if (ghostPart.IsInGroup("Eye"))
					{
						eyePool.Add((Body)ghostPart);
					}
					else if (ghostPart.IsInGroup("Beak"))
					{
						beakPool.Add((Body)ghostPart);
					}
				}

				switch (placePartId)
				{
					case 0:
						if (legPool.Count > 0)
						{
							ghostPart = legPool[0];
							legPool.RemoveAt(0);
							ghostPart.Visible = true;
						}
						else
						{
							ghostPart = (RigidBody2D)LegIns.Instantiate();
							AddChild(ghostPart);
						}
						break;
					case 1:
						if (bodyPool.Count > 0)
						{
							ghostPart = bodyPool[0];
							bodyPool.RemoveAt(0);
							ghostPart.Visible = true;
						}
						else
						{
							ghostPart = (RigidBody2D)BodyIns.Instantiate();
							AddChild(ghostPart);
						}
						break;
					case 2:
						if (eyePool.Count > 0)
						{
							ghostPart = eyePool[0];
							eyePool.RemoveAt(0);
							ghostPart.Visible = true;
						}
						else
						{
							ghostPart = (RigidBody2D)EyeIns.Instantiate();
							AddChild(ghostPart);
						}
						break;
					case 3:
						if (beakPool.Count > 0)
						{
							ghostPart = beakPool[0];
							beakPool.RemoveAt(0);
							ghostPart.Visible = true;
						}
						else
						{
							ghostPart = (RigidBody2D)BeakIns.Instantiate();
							AddChild(ghostPart);
						}
						break;
					default:
						break;
				}
				ghostPart.ZIndex = 10;
				ghostPart.CollisionLayer = 0;
				ghostPartId = placePartId;
			}
		}
		else
		{
			if (ghostPart != null)
			{
				if (ghostPart.IsInGroup("Leg"))
				{
					legPool.Add((Leg)ghostPart);
				}
				else if (ghostPart.IsInGroup("Body"))
				{
					bodyPool.Add((Body)ghostPart);
				}
				else if (ghostPart.IsInGroup("Eye"))
				{
					eyePool.Add((Body)ghostPart);
				}
				else if (ghostPart.IsInGroup("Beak"))
				{
					beakPool.Add((Body)ghostPart);
				}

				ghostPart.Visible = false;
				ghostPart = null;
				ghostPartId = -1;
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
			currentTool = 0;
			UpdateToolText();
			placePart = false;
			return;
		}

		currentTool = -1;
		UpdateToolText();

		placePart = true;

		placePartId = _partId;
	}

	public void BPScreenDown()
	{
		screenPressed = true;

		selectedCharacterRef = GameManager.instance.playerCharacter;

		var space = GetWorld2D().DirectSpaceState;

		var query = new PhysicsPointQueryParameters2D();

		query.CollisionMask = 16; // Visuals collision shapes

		query.Position = GetGlobalMousePosition();

		query.CollideWithAreas = true;

		var result = space.IntersectPoint(query);

		if (result.Count > 0)
		{
			int closest = 0;

			for (int i = 1; i < result.Count; i++)
			{
				if (((CollisionObject2D)result[i]["collider"]).ZIndex > ((CollisionObject2D)result[closest]["collider"]).ZIndex)
				{
					closest = i;
				}
			}

			if (currentTool == 1)
			{
				// Select the part
				CollisionObject2D selectedObject = (CollisionObject2D)result[closest]["collider"];

				if (selectedObject.IsInGroup("SelectArea"))
				{
					selectedPart = (RigidBody2D)selectedObject.GetParent();
				}
				else
				{
					selectedPart = (RigidBody2D)selectedObject;
				}
				selectedPart.Modulate = new Color(0.4f, 0.4f, 1.0f);

				toolOffset = selectedPart.GlobalPosition - GetGlobalMousePosition();

				moveOriginalPos = selectedPart.GlobalPosition;
			}
		}
	}

	public void BPScreenUp()
	{
		screenPressed = false;

		if (currentTool != 1)
			return;

		if (selectedPart != null)
		{
			selectedPart.Modulate = new Color(1f, 1f, 1f);

			// Get selected part type
			int typeId = 0;
			if (selectedPart.IsInGroup("Body"))
			{
				typeId = 1;
			}
			else if (selectedPart.IsInGroup("Leg"))
			{
				typeId = 0;
			}
			else if (selectedPart.IsInGroup("Eye"))
			{
				typeId = 2;
			}
			else if (selectedPart.IsInGroup("Beak"))
			{
				typeId = 3;
			}

			selectedCharacterRef = GameManager.instance.playerCharacter;

			var space = GetWorld2D().DirectSpaceState;

			var query = new PhysicsShapeQueryParameters2D();

			CircleShape2D shape = new CircleShape2D();

			shape.Radius = placeRadius[typeId];

			query.Shape = shape;

			query.CollisionMask = 8;

			placePos = GetGlobalMousePosition();

			query.Transform = new Transform2D(0, placePos);

			var result = space.IntersectShape(query);

			if (result.Count > 0)
			{
				if ((RigidBody2D)result[0]["collider"] != (RigidBody2D)selectedPart)
				{
					var children = selectedPart.GetChildren();

					Joint2D joint = null;

					foreach (var child in children)
					{
						if (child.IsInGroup("Joint"))
						{
							joint = (Joint2D)child;
						}
					}

					//joint.Position = selectedPart.GlobalPosition;
					joint.NodeA = selectedPart.GetPath();
					joint.NodeB = ((RigidBody2D)result[0]["collider"]).GetPath();

					selectedPart.GetParent().RemoveChild(selectedPart);
					((RigidBody2D)result[0]["collider"]).AddChild(selectedPart);
				}
				else
				{
					selectedPart.GlobalPosition = moveOriginalPos;
				}
			}
			else
			{
				selectedPart.GlobalPosition = moveOriginalPos;
			}
			selectedPart = null;
		}
	}

	public void BPScreen() // Button pressed "Screen"
	{
		if (placePart)
		{
			selectedCharacterRef = GameManager.instance.playerCharacter;

			var space = GetWorld2D().DirectSpaceState;

			var query = new PhysicsShapeQueryParameters2D();

			CircleShape2D shape = new CircleShape2D();

			shape.Radius = placeRadius[placePartId];

			query.Shape = shape;

			query.CollisionMask = 8;

			placePos = GetGlobalMousePosition();

			query.Transform = new Transform2D(0, placePos);

			var result = space.IntersectShape(query);

			if (result.Count > 0)
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

				if (placePartId == 2)
				{
					PlaceEye(part);
				}

				if (placePartId == 3)
				{
					PlaceBeak(part);
				}

				currentTool = 0;
			}
		}
		else if (currentTool == 3)
		{
			selectedCharacterRef = GameManager.instance.playerCharacter;

			var space = GetWorld2D().DirectSpaceState;

			var query = new PhysicsPointQueryParameters2D();

			query.CollisionMask = 16; // Visuals collision shapes

			query.Position = GetGlobalMousePosition();

			query.CollideWithAreas = true;

			var result = space.IntersectPoint(query);

			if (result.Count > 0)
			{
				int closest = 0;

				for (int i = 1; i < result.Count; i++)
				{
					if (((CollisionObject2D)result[i]["collider"]).ZIndex > ((CollisionObject2D)result[closest]["collider"]).ZIndex)
					{
						closest = i;
					}
				}

				// Select the part
				CollisionObject2D selectedObject = (CollisionObject2D)result[closest]["collider"];

				if (selectedObject.IsInGroup("SelectArea"))
				{
					selectedPart = (RigidBody2D)selectedObject.GetParent();
				}
				else
				{
					selectedPart = (RigidBody2D)selectedObject;
				}
				
				GD.Print("Delete time!");

				DeletePart(selectedPart);

				selectedPart = null;
			}
		}
		else if (currentTool == 4)
		{
			selectedCharacterRef = GameManager.instance.playerCharacter;

			var space = GetWorld2D().DirectSpaceState;

			var query = new PhysicsPointQueryParameters2D();

			query.CollisionMask = 32; // Visuals collision shapes

			query.Position = GetGlobalMousePosition();

			query.CollideWithAreas = true;

			var result = space.IntersectPoint(query);

			if (result.Count > 0)
			{
				int closest = 0;

				for (int i = 1; i < result.Count; i++)
				{
					if (((CollisionObject2D)result[i]["collider"]).ZIndex > ((CollisionObject2D)result[closest]["collider"]).ZIndex)
					{
						closest = i;
					}
				}

				// Select the part
				CollisionObject2D selectedObject = (CollisionObject2D)result[closest]["collider"];

				if (selectedObject.IsInGroup("SelectArea"))
				{
					selectedPart = (RigidBody2D)selectedObject.GetParent();
				}
				else
				{
					selectedPart = (RigidBody2D)selectedObject;
				}

				Paint(selectedPart);

				selectedPart = null;
			}
		}
	}

	void Paint(RigidBody2D selectedPart)
	{
		if (selectedPart.IsInGroup("MainPart"))
		{
			((Character)selectedPart).Paint(pickedColour);
		}
		else if (selectedPart.IsInGroup("Body"))
		{
			((Body)selectedPart).Paint(pickedColour);
		}
		else if (selectedPart.IsInGroup("Leg"))
		{
			selectedPart.Modulate = pickedColour;
		}
		else if (selectedPart.IsInGroup("Eye"))
		{
			selectedPart.Modulate = pickedColour;
		}
		else if (selectedPart.IsInGroup("Beak"))
		{
			selectedPart.Modulate = pickedColour;
		}
	}

	void DeletePart(RigidBody2D selectedPart)
	{
		var children = selectedPart.GetChildren();

		Joint2D joint = null;

		foreach (var child in children)
		{
			if (child.IsInGroup("Joint"))
			{
				joint = (Joint2D)child;
			}
		}

		selectedCharacterRef.pinJoints.Remove(joint);

		joint.QueueFree();

		if (selectedPart.IsInGroup("Body"))
		{
			foreach (var child in children)
			{
				if (child.IsInGroup("Character"))
				{
					DeletePart((RigidBody2D)child);
				}
			}

			selectedCharacterRef.bodyParts.Remove((Body)selectedPart);
			bodyPool.Add((Body)selectedPart);

			((Body)selectedPart).Paint(new Color(1, 1, 1, 1));
			selectedPart.Visible = false;
		}
		else if (selectedPart.IsInGroup("Leg"))
		{
			selectedCharacterRef.legs.Remove((Leg)selectedPart);
			legPool.Add((Leg)selectedPart);
			selectedPart.Modulate = new Color(1,1,1,1);
			selectedPart.Visible = false;
		}
		else if (selectedPart.IsInGroup("Eye"))
		{
			selectedCharacterRef.bodyParts.Remove((Body)selectedPart);
			eyePool.Add((Body)selectedPart);
			selectedPart.Modulate = new Color(1, 1, 1, 1);
			selectedPart.Visible = false;
		}
		else if (selectedPart.IsInGroup("Beak"))
		{
			selectedCharacterRef.bodyParts.Remove((Body)selectedPart);
			beakPool.Add((Body)selectedPart);
			selectedPart.Modulate = new Color(1, 1, 1, 1);
			selectedPart.Visible = false;
		}

		selectedPart.GetParent().RemoveChild(selectedPart);
		AddChild(selectedPart);
	}

	void PlaceBody(RigidBody2D _part)
	{
		Body body;
		body = (Body)BodyIns.Instantiate();
		_part.AddChild(body);
		body.Position = placePos - _part.GlobalPosition;
		body.SetOrigin();
		selectedCharacterRef.bodyParts.Add(body);

		PinJoint2D joint = new PinJoint2D();
		body.AddChild(joint);
		joint.NodeA = _part.GetPath();
		joint.NodeB = body.GetPath();

		joint.Position = (_part.GlobalPosition - body.GlobalPosition) / 2; //new Vector2(0,0);
		joint.DisableCollision = false;
		//joint.Length = (_part.GlobalPosition - body.GlobalPosition).Length();
		//joint.Stiffness = 64;
		//joint.Bias = 0.9f;
		//joint.Damping = 16f;
		joint.AddToGroup("Joint");
		selectedCharacterRef.pinJoints.Add(joint);
	}

	void PlaceEye(RigidBody2D _part)
	{
		Body eye;
		eye = (Body)EyeIns.Instantiate();
		_part.AddChild(eye);
		eye.Position = placePos - _part.GlobalPosition;
		eye.SetOrigin();
		selectedCharacterRef.bodyParts.Add(eye);

		PinJoint2D joint = new PinJoint2D();
		eye.AddChild(joint);
		joint.NodeA = _part.GetPath();
		joint.NodeB = eye.GetPath();

		joint.Position = new Vector2(0,0);
		joint.DisableCollision = false;

		joint.AddToGroup("Joint");
		selectedCharacterRef.pinJoints.Add(joint);
	}

	void PlaceBeak(RigidBody2D _part)
	{
		Body beak;
		beak = (Body)BeakIns.Instantiate();
		_part.AddChild(beak);
		beak.Position = placePos - _part.GlobalPosition;
		beak.SetOrigin();
		selectedCharacterRef.bodyParts.Add(beak);
		//selectedCharacterRef.decorParts.Add(beak);

		FixedJoint2D joint = new FixedJoint2D();
		beak.AddChild(joint);

		//joint.NodeA = _part.GetPath();
		//joint.NodeB = beak.GetPath();

		joint.Position = new Vector2(0,0);
		joint.DisableCollision = false;

		joint.ConnectNodes(_part, beak);

		joint.AddToGroup("Joint");
		selectedCharacterRef.pinJoints.Add(joint);
	}

	void PlaceLeg(RigidBody2D _part, bool _backLeg)
	{
		var leg = (Leg)LegIns.Instantiate();
		leg.Position = placePos - _part.GlobalPosition;
		_part.AddChild(leg);
		selectedCharacterRef.legs.Add(leg);

		PinJoint2D joint = new PinJoint2D();
		leg.AddChild(joint);
		joint.NodeA = _part.GetPath();
		joint.NodeB = leg.GetPath();

		joint.Softness = 0.1f;

		joint.AddToGroup("Joint");
		selectedCharacterRef.pinJoints.Add(joint);

		if (_backLeg)
		{
			leg.SetBackLeg();
		}

		leg.SetCharacter(selectedCharacterRef);
		leg.attached = true;
	}

	public void BPPlay()
	{
		selectedCharacterRef.Play();

		hud.Visible = false;

		GameManager.instance.SetMode(Mode.PlayMode);
	}

	public void BPToolSelected(int _toolId)
	{
		currentTool = _toolId;

		placePart = false;

		UpdateToolText();
	}

	void UpdateToolText()
	{
		switch (currentTool)
		{
			case -1:
				toolText.Text = "Current tool: Add part";
				break;
			case 0:
				toolText.Text = "Current tool: Select";
				break;
			case 1:
				toolText.Text = "Current tool: Move";
				break;
			case 2:
				toolText.Text = "Current tool: Rotate";
				break;
			case 3:
				//toolText.Text = "Current tool: Scale";
				toolText.Text = "Current tool: Delete";
				break;
			case 4:
				toolText.Text = "Current tool: Paint";
				break;
		}
	}

	public void ClearAll()
	{
		var children = selectedCharacterRef.GetChildren();

		foreach (var child in children)
		{
			if (child.IsInGroup("Character"))
			{
				DeletePart((RigidBody2D)child);
			}
		}
	}

	public void ColourPicked(Color color)
	{
		pickedColour = color;
	}
}
