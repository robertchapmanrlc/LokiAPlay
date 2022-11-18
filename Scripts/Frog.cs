using Godot;
using System;

public class Frog : KinematicBody2D
{

	private const float FRICTION = 0.4f;
	private const float GRAVITY = 9f;
	private const float TERMINAL_VELOCITY = 225f;
	private const float JUMP_FORCE = -150f;
	private const float LEAP_FORCE = -200f;
	private const int JUMP_DISTANCE = 75;
	private const int LEAP_DISTANCE = 25;
	
	private bool grounded, canJump, hitWall, needHighJump;
	
	[Export]
	private int direction = -1;
	
	private String animation = "idle";
	private Vector2 velocity = new Vector2();
	
	private RandomNumberGenerator random;
	private AnimatedSprite sprite;
	private Timer jumpTimer;
	private RayCast2D wallRayCast, jumpDistanceCast, trappedCast1, trappedCast2;
	private AudioStreamPlayer2D jump1, jump2;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		
		canJump = false;
		hitWall = false;
		needHighJump = false;
		sprite = GetNode<AnimatedSprite>("Sprite");
		jumpTimer = GetNode<Timer>("JumpTimer");
		wallRayCast = GetNode<RayCast2D>("WallDetector");
		jumpDistanceCast = GetNode<RayCast2D>("viewableGroundCast");
		trappedCast1 = GetNode<RayCast2D>("trapedCasts/cast1");
		trappedCast2 = GetNode<RayCast2D>("trapedCasts/cast2");
		jumpDistanceCast.SetCastTo(new Vector2(0, 40f));
		jumpDistanceCast.SetPosition(new Vector2(40 * direction, 0));
		jump1 = GetNode<AudioStreamPlayer2D>("Sounds/Jump1");
		jump2 = GetNode<AudioStreamPlayer2D>("Sounds/Jump2");
		
		float randomNumber = (float)GD.RandRange(2f, 3f);
		jumpTimer.WaitTime = randomNumber;
		if(direction != -1){
			sprite.FlipH = !sprite.FlipH;
			wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
			jumpDistanceCast.SetPosition(new Vector2(40 * direction, 0));
		}
		
		random = new RandomNumberGenerator();
	}
	
	public override void _PhysicsProcess(float delta){
		
		getBools();
		if(grounded && jumpTimer.IsStopped() && velocity.x == 0){
			jumpTimer.Start();
		} else if (grounded && velocity.x != 0) {
			velocity.x = Lerp(velocity.x, 0, FRICTION);
		}

		if(canJump){
			canJump = false;
			if(!needHighJump){
				velocity.y = JUMP_FORCE;
				velocity.x = JUMP_DISTANCE * direction;
			} else {
				velocity.y = LEAP_FORCE;
				velocity.x = LEAP_DISTANCE * direction;
				needHighJump = false;
			}
			
		}

		if(hitWall || (!jumpDistanceCast.IsColliding() && grounded)){
			direction *= -1;
			wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
			jumpDistanceCast.SetPosition(new Vector2(40 * direction, 0));
			hitWall = false;
			sprite.FlipH = !sprite.FlipH;
		}

		velocity.y = Math.Min(velocity.y + GRAVITY, TERMINAL_VELOCITY);

		velocity = MoveAndSlide(velocity, Vector2.Up);
		
		_assign_animation();
	}
	
	private void getBools(){
		grounded = IsOnFloor();
		hitWall = wallRayCast.IsColliding() && !grounded;
		needHighJump = trappedCast1.IsColliding() && trappedCast2.IsColliding();
	}
	
	private void _assign_animation(){
		animation = "idle";
		if(!grounded && velocity.y < 0){
			animation = "jumping";
		} else if (!grounded) {
			animation = "falling";
		}
		sprite.Play(animation);
	}
	
	private float Lerp(float firstFloat, float secondFloat, float by){
		return firstFloat * (1 - by) + secondFloat * by;
	}

	private void _on_JumpTimer_timeout(){
		canJump = true;
		if(random.RandfRange(0,1f) >= 0.5f){
			jump1.Play();
		}else{
			jump2.Play();
		}
	}
	
	private void _on_HitBox_body_entered(object body)
	{
		GetTree().ReloadCurrentScene();
	}
}
