using Godot;
using System;

public partial class Mob : RigidBody2D
{
	public override void _Ready()
	{
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
		animatedSprite2D.Play(mobTypes[ GD.Randi() % mobTypes.Length ]);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//connected nodes
	private void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}






