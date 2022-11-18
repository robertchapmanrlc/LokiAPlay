using Godot;
using System;

public class CheckPoint : Node2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var g = (Global)GetNode("/root/Global");
		if(g.getCheckPoint() != GetGlobalPosition()){
			GetNode<Sprite>("CheckPointFlagReached").Visible = false;
		} else {
			GetNode<Sprite>("CheckPointFlag").Visible = false;
		}
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	private void _on_Area2D_body_entered(object body)
	{
		var g = (Global)GetNode("/root/Global");
		if(GetNode<Sprite>("CheckPointFlag").Visible){
			g.setCheckPoint(GetGlobalPosition());
			GetNode<Sprite>("CheckPointFlag").Visible = false;
			GetNode<Sprite>("CheckPointFlagReached").Visible = true;
		}
	}
}

