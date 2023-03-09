using Godot;
using System;

public enum PartData { Leg = 1, Body = 2 }

public partial class ButtonData : Button
{
	[Export] PartData partData;

	public PartData GetPartData()
	{ 
		return partData; 
	}
}
