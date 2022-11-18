using Godot;
using System;

public class Slime : KinematicBody2D
{
	
	private const int SPEED = 15;
	private const int GRAVITY = 7;
	private const int TERMINAL_VELOCITY = 225;
	
	[Export]
	private int direction = -1;
	
	private Vector2 velocity = new Vector2();
	
	private AnimatedSprite sprite;
	private RayCast2D floorChecker;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite>("Sprite");
		floorChecker = GetNode<RayCast2D>("floorChecker");
		floorChecker.SetCastTo(new Vector2(4 * direction, 10f));
		if(direction != -1){
			sprite.FlipH = !sprite.FlipH;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(float delta)
	{
		if(IsOnWall() || !floorChecker.IsColliding()){
			direction *= -1;
			floorChecker.SetCastTo(new Vector2(4 * direction, 10f));
			sprite.FlipH = !sprite.FlipH;
		}
		
		velocity.y = Math.Min(velocity.y + GRAVITY, TERMINAL_VELOCITY);
		velocity.x = SPEED * direction;
		velocity = MoveAndSlide(velocity, Vector2.Up);
	}
	
	private void _on_BottomArea_body_entered(object body){
		GetTree().ReloadCurrentScene();
	}
	
	private void _on_TopArea_body_entered(object body){
		GetTree().ReloadCurrentScene();
	}
	
	private void _on_SidesArea_body_entered(object body){
		GetTree().ReloadCurrentScene();
	}
}
