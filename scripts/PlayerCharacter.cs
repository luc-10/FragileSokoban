using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
	[Export] public float Speed = 256f;

	public override void _PhysicsProcess(double delta)
	{
		GetPlayerInput();
		if (MoveAndSlide())
		{
			ResolveCollisions();
		}
	}

	private void GetPlayerInput()
	{
		Vector2 dir = Input.GetVector("ui_left","ui_right","ui_up","ui_down");
		Velocity = dir * Speed;
	}

	private void ResolveCollisions()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var body = (Box) collision.GetCollider();
			if (body != null)
			{
				Vector2 dir = Input.GetVector("ui_left","ui_right","ui_up","ui_down");
				body.MoveBox(dir);
			}
		}
	}
}
