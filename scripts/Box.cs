using Godot;
using System;

public partial class Box : CharacterBody2D
{
	[Export] public float TileSize = 64f;
	[Export] public float MoveTime = 0.1f;

	private bool _moving = false;

	public override void _PhysicsProcess(double delta)
	{
	}

	public void MoveBox(Vector2 dir)
	{
		if (dir.X != 0 && dir.Y != 0)
		{
			return;
		}

		if (_moving)
		{
			return;
		}
		
		var collision = GetNode<CollisionShape2D>("CollisionShape2D");
		collision.Disabled = true;
		
		Vector2 target = Position + dir * TileSize;
		_moving = true;
		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", target, MoveTime);
		tween.Finished += () =>
		{
			_moving = false;
			collision.Disabled = false;
		};
		
		
	}
}
