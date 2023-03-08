using Godot;
using System;

public enum SceneName
{
	MENU,
	GAME,
}

public partial class MasterManager : Node
{
	[Export]
	public PackedScene game;

	[Export]
	public PackedScene menu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Randomize();

		// Load the first scene
		AddChild(game.Instantiate());
	}

	public void ChangeScene(SceneName _sceneName)
	{
		GetChild(0).QueueFree();

		PackedScene nextScene = new PackedScene();

		if (_sceneName == SceneName.MENU)
		{
			nextScene = menu;
		}
		else if (_sceneName == SceneName.GAME)
		{
			nextScene = game; 
		}
		
		AddChild(nextScene.Instantiate());
	}

	public override void _Process(double delta)
	{
		
	}
}
