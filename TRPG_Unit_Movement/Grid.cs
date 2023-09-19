using Godot;
using System;
public partial class Grid : Node
{
	[Export]
	public Vector2 size = new Vector2(20,20);
	[Export]
	public Vector2 cellSize = new Vector2(80,80);

	private Vector2 HalfCellSize()
	{
		return cellSize / 2;
	}

	//returns position of cell's center in pixels
	public Vector2 CalculateMapPosition(Vector2 gridPosition)
	{
		return gridPosition * cellSize + HalfCellSize();
	}
	
	//returns coordinates of the cell given the position on the map
	public Vector2 CalculateGridCoordinates(Vector2 mapPosition)
	{
		return (mapPosition / cellSize).Floor();
	}

	//checks that cell is witihin the grid; used to prevent cursor/units from leaving grid
	public bool IsWithinBounds(Vector2 cellCoordinates)
	{
		bool insideX = cellCoordinates.X >= 0 && cellCoordinates.X < size.X;
		bool insideY = cellCoordinates.Y >= 0 && cellCoordinates.Y < size.Y;
		return insideX && insideY;
	}

	//clamps a vector2 by coordinates, rather than by total vector length
	public Vector2 Clamp(Vector2 gridPosition)
	{
		Vector2 clamppedGridPosition = gridPosition;
		clamppedGridPosition.X = Mathf.Clamp(clamppedGridPosition.X, 0f, size.X - 1.0f); 
		clamppedGridPosition.Y = Mathf.Clamp(clamppedGridPosition.Y, 0f, size.Y - 1.0f);
		return clamppedGridPosition;
	}

	public int AsIndex(Vector2 cell)
	{
		return (int)(cell.X + size.X * cell.Y);
	}

}
