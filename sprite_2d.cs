using Godot;
using System;

public partial class sprite_2d : Sprite2D
{
	private int _speed = 400;
	private float _angularSpeed = Mathf.Pi;
	
	public override void _Process(double delta) {
		float direction = 0;
		if (Input.IsActionPressed("ui_left")) {
			direction = -1;
		}
		if (Input.IsActionPressed("ui_right")) {
			direction = 1;
		}
		
		Rotation += _angularSpeed * direction * (float)delta;
		
		Vector2 velocity = Vector2.Zero;
		if (Input.IsActionPressed("ui_up")) {
			velocity = Vector2.Up.Rotated(Rotation) * _speed;	
		}
		
		Position += velocity * (float)delta;
	}
}
