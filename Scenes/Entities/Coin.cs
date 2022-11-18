using Godot;
using System;

public class Coin : Sprite
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		String level = GetTree().GetCurrentScene().GetName();
		var g = (Global)GetNode("/root/Global");
		if(level.Substring(5) == "" + g.getCoinCount()){
			QueueFree();
		}
	}

	private void _on_Area2D_body_entered(object body)
	{
		var g = (Global)GetNode("/root/Global");
		g.incrementCoin();
		QueueFree();
	}

}
