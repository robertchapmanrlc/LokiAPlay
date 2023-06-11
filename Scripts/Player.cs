using Godot;
using System;

public class Player : KinematicBody2D
{
	private bool grounded, walking, jumping, on_wall, wall_jumping, wall_sliding;
	
	private const float ACCELERATION = 10f;
	private const float MAX_SPEED = 60f;
	private const float FRICTION = 0.4f;
	private const float GRAVITY = 9f;
	private const float TERMINAL_VELOCITY = 225f;
	private const float JUMP_FORCE = -190f;
	
	private float jumpTimer, jumpBufferWindow, ledgeJumpTimer, ledgeJumpBufferWindow;
	
	private Vector2 leftFacingHitBox, rightFacingHitBox;
	
	private String animation = "idle";
	
	private RandomNumberGenerator random;
	private RayCast2D rightRayCast, leftRayCast;
	private CollisionShape2D hitbox;
	private AnimatedSprite sprite;
	private AudioStreamPlayer jump, wallKick1, wallKick2;
	
	private Vector2 velocity = new Vector2();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var g = (Global)GetNode("/root/Global");
		if(g.getCheckPoint() != new Vector2()){
			SetGlobalPosition(g.getCheckPoint());
		}
		jumpBufferWindow = 0.05f;
		ledgeJumpBufferWindow = 0.1f;
		
		leftFacingHitBox = new Vector2(-0.5f, 0.5f);
		rightFacingHitBox = new Vector2(0.5f, 0.5f);
		
		animation = "idle";
		
		rightRayCast = GetNode<RayCast2D>("rightRayCast");
		leftRayCast = GetNode<RayCast2D>("leftRayCast");
		sprite = GetNode<AnimatedSprite>("Sprites");
		hitbox = GetNode<CollisionShape2D>("HitBox");
		jump = GetNode<AudioStreamPlayer>("Sounds/Jump");
		wallKick1 = GetNode<AudioStreamPlayer>("Sounds/WallKick1");
		wallKick2 = GetNode<AudioStreamPlayer>("Sounds/WallKick2");
		random = new RandomNumberGenerator();
	}
	
	public void _getBools(){
		grounded = IsOnFloor();
		on_wall = IsOnWall() || rightRayCast.IsColliding() || leftRayCast.IsColliding();
		walking = (velocity.x > 1 || velocity.x < -1) && velocity.y == 0;
		wall_sliding = velocity.y > 0 && on_wall && !grounded && (Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right"));
	}
	
	public void _resetJumpBools(){
		if(jumping && velocity.y >= 0){
			jumping = false;
		}
		if(velocity.y > 0){
			wall_jumping = false;
		}
	}
	
	public override void _PhysicsProcess(float delta){
		
		_getBools();
		_resetJumpBools();
		
		_horizontal_input();
		_handling_jumping(delta);
		_handling_wall_kick();
		
		velocity.y = Math.Min(velocity.y + GRAVITY, TERMINAL_VELOCITY);
		
		velocity = MoveAndSlide(velocity, Vector2.Up);
		
		_assign_animation();
	}
	
	public void _horizontal_input(){
		if(Input.IsActionPressed("ui_left") && !wall_jumping){
			if(wall_sliding){
				sprite.FlipH = false;
				hitbox.Position = rightFacingHitBox;
			} else {
				sprite.FlipH = true;
				hitbox.Position = leftFacingHitBox;
			}
			velocity.x = Math.Max(velocity.x - ACCELERATION, -MAX_SPEED);
		} else if(Input.IsActionPressed("ui_right") && !wall_jumping){
			velocity.x = Math.Min(velocity.x + ACCELERATION, MAX_SPEED);
			if(wall_sliding){
				sprite.FlipH = true;
				hitbox.Position = leftFacingHitBox;
			} else {
				sprite.FlipH = false;
				hitbox.Position = rightFacingHitBox;
			}
		} else if (!wall_jumping) {
			velocity.x = Lerp(velocity.x, 0, FRICTION);
		}
	}
	
	public void _handling_jumping(float delta){
		
		jumpTimer -= delta;
		if(jumpTimer <= -5){
			jumpTimer = -5;
		}
		
		ledgeJumpTimer -= delta;
		if(ledgeJumpTimer <= -5){
			ledgeJumpTimer = -5;
		}
		
		if(grounded){
			ledgeJumpTimer = ledgeJumpBufferWindow;
		}
		
		if(Input.IsActionJustPressed("jump")){
			jumpTimer = jumpBufferWindow;
		}

		if(Input.IsActionJustReleased("jump") && velocity.y < JUMP_FORCE/2){
			velocity.y = JUMP_FORCE/2;
		}
		
		if(jumpTimer > 0 && ledgeJumpTimer > 0){
			jumping = true;
			velocity.y = JUMP_FORCE;
			if(!jump.IsPlaying())
				jump.Play();
		}
	}
	
	public void _handling_wall_kick(){
		if(wall_sliding && Input.IsActionJustPressed("jump")){
			wall_jumping = true;
			if(random.RandfRange(0,1f) >= 0.5f && !wallKick1.IsPlaying()){
				wallKick1.Play();
			} else if (random.RandfRange(0,1f) < 0.5f && !wallKick2.IsPlaying()) {
				wallKick2.Play();
			}
			if(Input.IsActionPressed("ui_right") && rightRayCast.IsColliding()){
				velocity.x = -75;
				velocity.y = (float)(JUMP_FORCE * 1.4);
			} else if(Input.IsActionPressed("ui_left") && leftRayCast.IsColliding()){
				velocity.x = 75;
				velocity.y = (float)(JUMP_FORCE * 1.35);
			}
		}
	}
	
	public void _assign_animation(){
		animation = "idle";
		if(!grounded){
			if(wall_sliding){
				animation = "wallSlide";
			} else if (wall_jumping) {
				animation = "wallKick";
			} else if(jumping){
				animation = "jumping";
			} else {
				animation = "falling";
			}
		} else if (grounded) {
			if(walking){
				animation = "walking";
			}
		}
		sprite.Play(animation);
	}
	
	private float Lerp(float firstFloat, float secondFloat, float by){
		return firstFloat * (1 - by) + secondFloat * by;
	}
}
