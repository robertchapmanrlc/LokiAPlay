using Godot;
using System;

public class Pit : Area2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	private void _on_Pit_body_entered(object body)
	{
		GetTree().ReloadCurrentScene();
	}	
}
