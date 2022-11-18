using Godot;
using System;

public class Flag : StaticBody2D
{
	private void _on_PlayerDetectionArea_body_entered(object body)
	{
		String name = GetTree().GetCurrentScene().GetName();
		var g = (Global)GetNode("/root/Global");
		if(name == "Level1"){
			g.setCheckPoint(new Vector2());
			GetTree().ChangeScene("res://Scenes/Levels/Level2.tscn");
		} else if (name == "Level2") {
			g.setCheckPoint(new Vector2());
			GetTree().ChangeScene("res://Scenes/Levels/Level3.tscn");
		} else if (name == "Level3" && g.getCoinCount() == 3){
			g.setCheckPoint(new Vector2());
			GetTree().ChangeScene("res://Scenes/Levels/SecretLevel.tscn");
		} else {
			GetTree().ChangeScene("res://Scenes/Levels/Credits.tscn");
		}
		
	}
}
