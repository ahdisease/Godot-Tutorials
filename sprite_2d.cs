using Godot;
using System;

public partial class sprite_2d : Sprite2D
{
//	The following is the format to add custom events:
//	[Signal]
//	public delegate void HealthDepletedEventHandler();
//		This is not appearing the node tab of the inspector; I'm not sure if this is because I'm using C# or if it's a limitation of Sprite2D object somehow

	
	private int _speed = 400;
	private float _angularSpeed = Mathf.Pi;
	
	public override void _Ready() 
	{
		Timer timer = GetNode<Timer>("Timer");
		timer.Timeout += OnTimerTimeout;
	}
	
	public override void _Process(double delta) 
	{
//		commented out for signal tutorial
//		float direction = 0;
//		if (Input.IsActionPressed("ui_left")) {
//			direction = -1;
//		}
//		if (Input.IsActionPressed("ui_right")) {
//			direction = 1;
//		}
//
//		Rotation += _angularSpeed * direction * (float)delta;
//
//		Vector2 velocity = Vector2.Zero;
//		if (Input.IsActionPressed("ui_up")) {
//			velocity = Vector2.Up.Rotated(Rotation) * _speed;	
//		}
//
//		Position += velocity * (float)delta;


		Rotation += _angularSpeed * (float)delta;
		var velocity = Vector2.Up.Rotated(Rotation) * _speed;
		Position += velocity * (float)delta;
	}
	
	private void OnButtonPressed()
	{
		SetProcess(!IsProcessing());
	}
	
	
	private void OnTimerTimeout()
	{
		Visible = !Visible;
	}
}






