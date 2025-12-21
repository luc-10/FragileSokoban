using Godot;
using System;

public partial class LevelManager : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export] public NodePath PlayerPath;
	private CharacterBody2D? player;

	[Export] public NodePath LevelContainerPath;
	private Node? levelContainer;
	
	private Node? _currentLevel;
	
	public override void _Ready()
	{
		player = GetNode<CharacterBody2D>(PlayerPath);
		levelContainer = GetNode<Node>(LevelContainerPath);
		
		LoadLevel(0);
	}

	public void LoadLevel(int index)
	{
		if (_currentLevel != null)
		{
			_currentLevel.SetProcess(false);
		}

		_currentLevel = levelContainer.GetChild(index);
		
		_currentLevel.SetProcess(true);

		var spawn = _currentLevel.GetNode<Marker2D>("PlayerStart");
		if (spawn == null)
		{
			GD.Print("Err", _currentLevel.Name);
		}
		if (player != null && spawn != null)
		{
			player.GlobalPosition = spawn.Position;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
