using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
	[Export] public float Speed = 256f;
	[Export] public int TileSize = 64;
	[Export] private float MoveTime = 0.2f;
	private bool _moving = false;

	public override void _Ready()
	{
		
	}
	public override void _PhysicsProcess(double delta)
	{
		var dir = getInput();
		if (_moving || dir == Vector2.Zero)
		{
			return;
		}

		var targetPos = Position + TileSize * dir;
		_moving = true;
		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", targetPos, MoveTime);
		tween.Finished += () =>
		{
			_moving = false;
		};

	}

	private Vector2 getInput()
	{
		Vector2 dir = Vector2.Zero;
		if (Input.IsActionJustPressed("ui_up"))
		{
			dir = Vector2.Up;
		} else if (Input.IsActionJustPressed("ui_down"))
		{
			dir = Vector2.Down;
		} else if (Input.IsActionJustPressed("ui_right"))
		{
			dir = Vector2.Right;
		} else if (Input.IsActionJustPressed("ui_left"))
		{
			dir = Vector2.Left;
		}

		return dir;
	}
}


