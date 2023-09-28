using Godot;
using System;
[GlobalClass]
public partial class Grid : Resource
{
	[Export]
	public Vector2I size = new Vector2I(20,20);
	[Export]
	public Vector2I cellSize = new Vector2I(80,80);

	private Vector2I HalfCellSize()
	{
		return cellSize / 2;
	}

	//returns position of cell's center in pixels
	public Vector2I CalculateMapPosition(Vector2I cell)
	{
		return cell * cellSize + HalfCellSize();
	}
	
	//returns coordinates of the cell given the position on the map
	public Vector2I CalculateGridCoordinates(Vector2I mapPosition)
	{
		Vector2I gridcoordinates = mapPosition / cellSize;
		return gridcoordinates;
	}

    public Vector2I CalculateGridCoordinates(Vector2 mapPosition)
    {
        Vector2 gridcoordinates = mapPosition / cellSize;
        return new Vector2I(Mathf.FloorToInt(gridcoordinates.X), Mathf.FloorToInt(gridcoordinates.Y));
    }

    //checks that cell is witihin the grid; used to prevent cursor/units from leaving grid
    public bool IsWithinBounds(Vector2I cellCoordinates)
	{
		bool insideX = cellCoordinates.X >= 0 && cellCoordinates.X < size.X;
		bool insideY = cellCoordinates.Y >= 0 && cellCoordinates.Y < size.Y;
		return insideX && insideY;
	}

	//clamps a vector2 by coordinates, rather than by total vector length
	public Vector2I Clamp(Vector2I gridPosition)
	{
		Vector2I clamppedGridPosition = gridPosition;
		clamppedGridPosition.X = Mathf.Clamp(clamppedGridPosition.X, 0, size.X - 1); 
		clamppedGridPosition.Y = Mathf.Clamp(clamppedGridPosition.Y, 0, size.Y - 1);
		return clamppedGridPosition;
	}

    public Vector2I Clamp(Vector2 gridPosition)
    {
        Vector2I clamppedGridPosition = new Vector2I(Mathf.FloorToInt(gridPosition.X), Mathf.FloorToInt(gridPosition.Y));
        clamppedGridPosition.X = Mathf.Clamp(clamppedGridPosition.X, 0, size.X - 1);
        clamppedGridPosition.Y = Mathf.Clamp(clamppedGridPosition.Y, 0, size.Y - 1);
        return clamppedGridPosition;
    }

    public long AsIndex(Vector2I cell)
	{
		return (cell.X + size.X * cell.Y);
	}

}
