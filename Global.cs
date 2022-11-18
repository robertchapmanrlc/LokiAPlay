using Godot;
using System;

public class Global : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	private int coinCount = 0;
	private Vector2 lastCheckPoint = new Vector2();

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

	public void incrementCoin(){
		coinCount += 1;
	}
	
	public int getCoinCount(){
		return coinCount;
	}
	
	public void setCheckPoint(Vector2 position){
		lastCheckPoint = position;
	}
	
	public Vector2 getCheckPoint(){
		return lastCheckPoint;
	}
}
