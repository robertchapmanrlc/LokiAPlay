using Godot;
using System;

public class Golem : KinematicBody2D
{
	private const int SPEED = 12;
	private const int GRAVITY = 7;
	private const int TERMINAL_VELOCITY = 225;
	
	[Export]
	private int direction = -1;
	
	private bool playerDetected = false;
	private String animation = "idle";
	private Vector2 velocity = new Vector2();
	
	private RandomNumberGenerator random;
	private AnimatedSprite sprite;
	private Timer startWalkTimer;
	private Timer stopWalkingTimer;
	private Timer soundEffectTimer;
	private RayCast2D wallRayCast, floorChecker;
	private AudioStreamPlayer2D loudStep, louderStep, quietStep, quieterStep;
	
	private Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		random = new RandomNumberGenerator();
		sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		startWalkTimer = GetNode<Timer>("StartWalkTimer");
		stopWalkingTimer = GetNode<Timer>("StopWalkingTimer");
		soundEffectTimer = GetNode<Timer>("SoundEffectTimer");
		wallRayCast = GetNode<RayCast2D>("wallRayCast");
		floorChecker = GetNode<RayCast2D>("floorChecker");
		floorChecker.SetPosition(new Vector2(4 * direction, 0));
		loudStep = GetNode<AudioStreamPlayer2D>("Sounds/LoudStep");
		louderStep = GetNode<AudioStreamPlayer2D>("Sounds/LoudestStep");
		quietStep = GetNode<AudioStreamPlayer2D>("Sounds/QuietStep");
		quieterStep = GetNode<AudioStreamPlayer2D>("Sounds/QuietestStep");
		if(direction == 1){
			sprite.FlipH = !sprite.FlipH;
			wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
		}
		
		float randomNumber = (float)GD.RandRange(2f, 3f);
		startWalkTimer.WaitTime = randomNumber;
		startWalkTimer.Start();
		
		randomNumber = (float)GD.RandRange(2f, 3f);
		stopWalkingTimer.WaitTime = randomNumber;
	}

	public override void _PhysicsProcess(float delta){
		if(wallRayCast.IsColliding() || !floorChecker.IsColliding()){
			direction *= -1;
			velocity.x = SPEED * direction;
			sprite.FlipH = !sprite.FlipH;
			wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
			floorChecker.SetPosition(new Vector2(4 * direction, 0));
		}
		
		chase_player();
		velocity.y = Math.Min(velocity.y + GRAVITY, TERMINAL_VELOCITY);
		velocity = MoveAndSlide(velocity, Vector2.Up);
		
		if(velocity.x == 0){
			soundEffectTimer.Stop();
		}
		
		_assign_animation();
	}
	
	private void _assign_animation(){
		animation = "idle";
		if (playerDetected) {
			animation = "chasing";
		} else if(velocity.x != 0){
			animation = "walking";
			soundEffectTimer.SetWaitTime(0.8f);
		}
		
		sprite.Play(animation);
	}
	
	private void chase_player(){
		if(playerDetected){
			if(player.GlobalPosition.x > GlobalPosition.x){
				if(velocity.x < 0){
					direction *= -1;
					sprite.FlipH = !sprite.FlipH;
					wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
					floorChecker.SetPosition(new Vector2(4 * direction, 0));
				}
			} else {
				if(velocity.x > 0){
					direction *= -1;
					sprite.FlipH = !sprite.FlipH;
					wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
					floorChecker.SetPosition(new Vector2(4 * direction, 0));
				}
			}
			if(floorChecker.IsColliding()){
				velocity.x = SPEED * 4f * direction;
			} else {
				velocity.x = 0f;
			}
		}
	}
	
	private void _on_StartWalkTimer_timeout()
	{
		float randomNumber = (float)GD.RandRange(2f, 3f);
		if(randomNumber < 2.5f){
			direction *= -1;
			sprite.FlipH = !sprite.FlipH;
			wallRayCast.SetCastTo(new Vector2(-wallRayCast.GetCastTo().x, 0));
			floorChecker.SetPosition(new Vector2(4 * direction, 0));
		}
		velocity.x = SPEED * direction;
		stopWalkingTimer.Start();
		soundEffectTimer.Start();
	}
	
	private void _on_StopWalkingTimer_timeout()
	{
		velocity.x = 0;
		startWalkTimer.Start();
	}
	
	private void _on_HitBox_body_entered(object body)
	{
		GetTree().ReloadCurrentScene();
	}
	
	private void _on_LineOfSight_body_entered(object body)
	{
		if(body is Player){
			soundEffectTimer.SetWaitTime(0.6f);
			soundEffectTimer.Start();
			stopWalkingTimer.Stop();
			startWalkTimer.Stop();
			player = body as Player;
			playerDetected = true;
		}
	}
	
	private void _on_LineOfSight_body_exited(object body)
	{
		if(body is Player){
			player = body as Player;
			playerDetected = false;
			stopWalkingTimer.Stop();
			startWalkTimer.Stop();
			velocity.x = 0;
			startWalkTimer.Start();
		}
	}
	
	private void _on_SoundEffectTimer_timeout()
	{
		if (playerDetected){
			if(random.RandfRange(0,1f) >= 0.5f){
				loudStep.Play();
			} else {
				louderStep.Play();
			}
		} else if(!playerDetected && animation == "walking"){
			if(random.RandfRange(0,1f) >= 0.5f){
				quietStep.Play();
			} else {
				quieterStep.Play();
			}
		}
		soundEffectTimer.Start();
	}
}
