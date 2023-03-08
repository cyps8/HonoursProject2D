using Godot;
using System;

public enum Mode { PlayMode, EditMode }

public partial class GameManager : Node2D
{
	public static GameManager instance;

	[Export] PackedScene characterIns;

	public Character playerCharacter;

	Mode currentMode = Mode.EditMode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (instance == null)
		{
			instance = this;
		}

		// create character
		playerCharacter = (Character)characterIns.Instantiate();
		AddChild(playerCharacter);
		playerCharacter.Position = new Vector2(300, 300);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionPressed("Reset") && currentMode != Mode.EditMode)
        {
            currentMode = Mode.EditMode;

			CharacterCreator.instance.Reset();

			playerCharacter.Reset();
        }
    }

	public Mode GetMode() 
	{  
		return currentMode;
	}

	public void SetMode(Mode _newMode)
	{
		currentMode = _newMode;
	}
	
}
