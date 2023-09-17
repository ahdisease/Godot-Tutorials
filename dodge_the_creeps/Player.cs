using Godot;
using System;

public partial class Player : Area2D
{
	[Export]
	public int Speed {get; set;} = 400;
	public Vector2 ScreenSize;
	
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;

	}

	public override void _Process(double delta)
	{
		Vector2 currentMovement = DetectMovement(delta);
		AnimateMovement(currentMovement);
	}
	
	private Vector2 DetectMovement(double delta) {
		Vector2 velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up"))
		{
			velocity.Y -= 1;
		}

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			
			Position += velocity * (float)delta;
			Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
			);
		}
		return velocity;
	}
	
	private void AnimateMovement(Vector2 velocity) {
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		if (!(velocity.Length() > 0)) {
			animatedSprite2D.Stop();
		} 
		
		animatedSprite2D.Play();
		
		if (velocity.X != 0)
		{
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipV = false;
			
			animatedSprite2D.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite2D.Animation = "up";
			animatedSprite2D.FlipV = velocity.Y > 0;
		}
	}
}
