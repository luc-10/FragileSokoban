using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
	[Export] public float TileSize = 64f;
	[Export] public float Speed = 240f;

	public override void _PhysicsProcess(double delta)
	{
		GetPlayerInput();
		MoveAndSlide();
	}

	private void GetPlayerInput()
	{
		Vector2 dir = Input.GetVector("ui_left","ui_right","ui_up","ui_down");
		Velocity = dir * Speed;
	}
}
