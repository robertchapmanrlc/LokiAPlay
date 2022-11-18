using Godot;
using System;

public class Background : TextureRect
{
	
	private Player player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent<Node2D>().GetNode<Player>("Player");
	}

	public override void _PhysicsProcess(float delta){
		SetGlobalPosition(new Vector2(player.GetGlobalPosition().x - 105, player.GetGlobalPosition().y - 65));
	}
}
