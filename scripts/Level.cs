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
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fillGrid();
		printGrid();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		foreach (Vector2 cell in walls.GetUsedCells())
		{
			grid[(int)cell.X-rect.Position.X,(int)cell.Y-rect.Position.Y] = CellType.Wall;
		}
		
		
		// Put the boxes

		var boxes = GetTree().GetNodesInGroup("Boxes");
		foreach (Box box in boxes)
		{
			Vector2 cell = new Vector2(box.Position.X/Main.TileSize, box.Position.Y/Main.TileSize);
			grid[(int)cell.X-rect.Position.X,(int)cell.Y-rect.Position.Y] = CellType.Box;
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

	public bool canMoveTo(Vector2 pos, Vector2 dir)
	{
		if (getTargetCell(pos, dir) == CellType.Wall)
		{
			printGrid();
			return false;
		} else if (getTargetCell(dir,pos) == CellType.Box)
		{
			//What to do:
			//Find all boxes
			//Look for the ones that would be moving into walls
			//call removeBox on them
			//call canMoveTo on all the others
			//check if the player can move
			//profit?
			var boxes = GetTree().GetNodesInGroup("Boxes");
			List<Box> notAgainstWall = new List<Box>();
			foreach (Box box in boxes)
			{
				Vector2 boxPos = new Vector2(box.Position.X / Main.TileSize, box.Position.Y / Main.TileSize);
				if (getTargetCell(boxPos,dir) == CellType.Wall)
				{
					if (box.RemoveBox())
					{
						grid[(int)boxPos.X, (int)boxPos.Y] = CellType.Empty;
					}
				}
				else
				{
					notAgainstWall.Add(box);
				}
			}

			foreach (Box box in notAgainstWall)
			{
				box.MoveBox(dir);
				Vector2 boxPos = new Vector2(box.Position.X / Main.TileSize, box.Position.Y / Main.TileSize);
				
				(grid[(int)boxPos.X + (int)dir.X, (int)boxPos.Y + (int)dir.Y], grid[(int)boxPos.X, (int)boxPos.Y]) = (grid[(int)boxPos.X, (int)boxPos.Y],grid[(int)boxPos.X + (int)dir.X, (int)boxPos.Y + (int)dir.Y]);

				
			}

			return getTargetCell(pos, dir) == CellType.Empty;
			/*
			if (canMoveTo(target, dir))
			{
				//Move box at position = target
				(grid[(int)target.X + (int)dir.X, (int)target.Y + (int)dir.Y], grid[(int)target.X, (int)target.Y]) = (grid[(int)target.X, (int)target.Y],grid[(int)target.X + (int)dir.X, (int)target.Y + (int)dir.Y]);
				var boxes = GetTree().GetNodesInGroup("Boxes");

				// Look for all boxes, when it finds one that is at the same position as the one in the grid then move it
				foreach (Box box in boxes)
				{
					if (box.Position.X/Main.TileSize == target.X && box.Position.Y/Main.TileSize == target.Y)
					{
						box.MoveBox(dir);
						break;
					}
				}
				printGrid();
				return true;
			}
			else
			{
				printGrid();
				return false;
			}
			*/
		} 
		printGrid();
		return true;
	}

	private CellType getTargetCell(Vector2 pos, Vector2 dir)
	{
		var target = pos + dir;
		return grid[(int)target.X, (int)target.Y];
	}
}
