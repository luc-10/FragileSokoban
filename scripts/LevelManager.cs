using Godot;
using System;

public partial class LevelManager : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export] public NodePath PlayerPath;
	private PlayerCharacter? player;

	[Export] public NodePath LevelContainerPath;
	private Node? levelContainer;
	
	private Node? _currentLevel;

	private int index = 1;
	
	public override void _Ready()
	{
		player = GetNode<PlayerCharacter>(PlayerPath);
		levelContainer = GetNode<Node2D>(LevelContainerPath);
		
		LoadLevel();
	}

	public void LoadLevel()
	{
		if (_currentLevel != null)
		{
			_currentLevel.SetProcess(false);
		}
		
		_currentLevel?.QueueFree();
		_currentLevel = null;
		PackedScene scene = GD.Load<PackedScene>("res://levels/level."+index+".tscn");
		_currentLevel = scene.Instantiate();
		levelContainer.AddChild(_currentLevel);
		var spawn = _currentLevel.GetNode<Marker2D>("PlayerStart");
		if (spawn == null)
		{
			GD.Print("Err", _currentLevel.Name);
		}
		if (player != null && spawn != null)
		{
			player.GlobalPosition = spawn.Position;
			player.setCurrLevel((Level) _currentLevel);
		}
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("reset"))
		{
			LoadLevel();
		}
	}

	public void loadNextLevel()
	{
		index++;
		LoadLevel();
	}
	
	
}
