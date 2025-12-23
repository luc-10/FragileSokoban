using Godot;
using System;

public partial class Box : CharacterBody2D
{
	[Export] public float TileSize = 64f;
	[Export] public float MoveTime = 0.1f;

	private bool _moving = false;
	[Export] int hp = 5;

	private Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.RegionRect = new Rect2(Main.TileSize * (hp-1), 0, Main.TileSize, Main.TileSize);
	}

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

	//find a better name, returns true if the box is destroyed
	public bool RemoveBox()
	{
		hp--;
		sprite.RegionRect = new Rect2(Main.TileSize * (hp-1), 0, Main.TileSize, Main.TileSize);
		if (hp == 0)
		{
			QueueFree();
			return true;
		}

		return false;
	}
	
	
}
