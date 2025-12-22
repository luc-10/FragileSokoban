using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using FragileSokoban.scripts;

public partial class Level : Node2D
{
	[Export] public NodePath TmlWallsPath;
	[Export] public NodePath TmlBackgroundPath;
	public int Width, Height;
	
	public CellType[,] grid;
	private Dictionary<Vector2I, Box> coordsToBoxMap;
	private Dictionary<Box, Vector2I> boxToCoordsMap;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fillGrid();
		printGrid();
		fillMaps();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void fillMaps()
	{
		coordsToBoxMap = new();
		boxToCoordsMap = new();
		var boxes = GetTree().GetNodesInGroup("Boxes");
		foreach (Box box in boxes)
		{
			Vector2I boxPos = new Vector2I((int)box.Position.X / Main.TileSize, (int)box.Position.Y / Main.TileSize);
			coordsToBoxMap[boxPos] = box;
			boxToCoordsMap[box] = boxPos;
		}
	}

	private void fillGrid()
	{
		
		// Get size of the grid
		var bg= GetNode<TileMapLayer>(TmlBackgroundPath);
		var rect = bg.GetUsedRect();

		Width = rect.Size.X;
		Height = rect.Size.Y;
		
		grid = new CellType[Width, Height];
		
		
		// Fill it with empty slots
		for (int i = 0; i < Width; i++)
		{
			for (int j = 0; j < Height; j++)
			{
				grid[i,j]=CellType.Empty;
			}
		}
		
		// Put the walls
		
		var walls = GetNode<TileMapLayer>(TmlWallsPath);
		foreach (Vector2I cell in walls.GetUsedCells())
		{
			grid[cell.X-rect.Position.X,cell.Y-rect.Position.Y] = CellType.Wall;
		}
		
		
		// Put the boxes

		var boxes = GetTree().GetNodesInGroup("Boxes");
		foreach (Box box in boxes)
		{
			Vector2I cell = new Vector2I((int)box.Position.X/Main.TileSize, (int)box.Position.Y/Main.TileSize);
			grid[cell.X-rect.Position.X,cell.Y-rect.Position.Y] = CellType.Box;
		}
		
		
		
	}

	private void printGrid()
	{
		//Print (for debug)

		for (int i = 0; i < Width; i++)
		{
			var line = "";
			for (int j = 0; j < Height; j++)
			{
				if (grid[i,j] == CellType.Empty)
				{
					line += ".";
				}
				else if (grid[i,j] == CellType.Wall)
				{
					line += "W";
				}
				else
				{
					line += "B";
				}
			}

			GD.Print(line);
		}
		
	}

	public bool canMoveTo(Vector2I pos, Vector2I dir)
	{
		if (getTargetCell(pos, dir) == CellType.Wall)
		{
			printGrid();
			return false;
		} else if (getTargetCell(pos,dir) == CellType.Box)
		{
			var boxes = GetTree().GetNodesInGroup("Boxes");
			HashSet<Box> moved = new();
			foreach (Box box in boxes)
			{
				
				checkBoxMove(boxToCoordsMap[box],dir, moved);
				printGrid();
			}
			return getTargetCell(pos, dir) == CellType.Empty;
		} 
		printGrid();
		return true;
	}

	private void checkBoxMove(Vector2I pos, Vector2I dir, HashSet<Box> moved)
	{
		var box = coordsToBoxMap[pos];
		if (!moved.Contains(box))
		{
			//If wall don't move
			//If box try to move other box
			//If empty move
			if (getTargetCell(pos, dir) == CellType.Wall)
			{

				if (box.RemoveBox())
				{
					grid[pos.X, pos.Y] = CellType.Empty;
					coordsToBoxMap.Remove(boxToCoordsMap[box]);
					boxToCoordsMap.Remove(box);
				}
			} else if (getTargetCell(pos, dir) == CellType.Empty) // add goal eventually
			{
				box.MoveBox(dir);
				(grid[pos.X + dir.X, pos.Y + dir.Y], grid[pos.X, pos.Y]) = 
					(grid[pos.X, pos.Y],grid[pos.X + dir.X, pos.Y + dir.Y]);
				boxToCoordsMap[box] = new Vector2I(pos.X + dir.X, pos.Y + dir.Y);
				coordsToBoxMap.Remove(pos);
				coordsToBoxMap[new Vector2I(pos.X + dir.X, pos.Y + dir.Y)] = box;

			}
			else if (getTargetCell(pos, dir) == CellType.Box)
			{
				checkBoxMove(pos+dir,dir,moved);

				if (getTargetCell(pos, dir) == CellType.Empty)
				{
					box.MoveBox(dir);
					(grid[pos.X + dir.X, pos.Y + dir.Y], grid[pos.X, pos.Y]) = 
						(grid[pos.X, pos.Y],grid[pos.X + dir.X, pos.Y + dir.Y]);
					boxToCoordsMap[box] = new Vector2I(pos.X + dir.X, pos.Y + dir.Y);
					coordsToBoxMap.Remove(pos);
					coordsToBoxMap[new Vector2I(pos.X + dir.X, pos.Y + dir.Y)] = box;
				}
				else
				{
					if (box.RemoveBox())
					{
						
						grid[pos.X, pos.Y] = CellType.Empty;
						coordsToBoxMap.Remove(boxToCoordsMap[box]);
						boxToCoordsMap.Remove(box);
					}
				}
			}
			moved.Add(box);
			
		}
	}

	private CellType getTargetCell(Vector2I pos, Vector2I dir)
	{
		var target = pos + dir;
		return grid[target.X, target.Y];
	}
}
