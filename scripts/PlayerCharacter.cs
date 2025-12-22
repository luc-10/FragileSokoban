using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
	[Export] private float _moveTime = 0.1f;
	[Export] public NodePath LevelPath;
	private bool _moving = false;
	private Level _currLevel;

	public override void _Ready()
	{
	}

	public void setCurrLevel(Level level)
	{
		_currLevel = level;
	}
	public override void _PhysicsProcess(double delta)
	{
		var dir = getInput();
		Vector2I gridPos = new Vector2I((int)Position.X/Main.TileSize,(int)Position.Y/Main.TileSize);
		if (_moving || dir == Vector2.Zero || !_currLevel.canMoveTo(gridPos,dir))
		{
			return;
		}

		var targetPos = Position + Main.TileSize * dir;
		_moving = true;
		Tween tween = CreateTween();
		tween.TweenProperty(this, "position", targetPos, _moveTime);
		tween.Finished += () =>
		{
			_moving = false;
		};

	}

	private Vector2I getInput()
	{
		Vector2I dir = Vector2I.Zero;
		if (Input.IsActionJustPressed("move_up"))
		{
			dir = Vector2I.Up;
		} else if (Input.IsActionJustPressed("move_down"))
		{
			dir = Vector2I.Down;
		} else if (Input.IsActionJustPressed("move_right"))
		{
			dir = Vector2I.Right;
		} else if (Input.IsActionJustPressed("move_left"))
		{
			dir = Vector2I.Left;
		}

		return dir;
	}

}
