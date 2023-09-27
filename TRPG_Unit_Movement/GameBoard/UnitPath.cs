using Godot;
using System.Collections.Generic;

public partial class UnitPath : TileMap
{
	//resources
	[Export] Grid grid;

	//state variables
	Pathfinder _pathfinder;
	Vector2[] currentPath;		//cached from pathfinder, used for WalkAlong()

	private void Initialize(Vector2[] walkableCells)
	{
		_pathfinder = new Pathfinder(grid,walkableCells);
	}

	private void DrawPath(Vector2 cellStart, Vector2 cellEnd)
	{
		Clear();    //removes previous values on the tilemap

		//currentPath = _pathfinder.CalculatePointPath(cellStart, cellEnd);

		//var currentPath = new Godot.Collections.Array<Vector2> { new Vector2(4, 4), new Vector2(5, 4), new Vector2(5, 5), new Vector2(5, 6) };
		var currentPathI = new Godot.Collections.Array<Vector2I> { new Vector2I(4, 4), new Vector2I(5, 4), new Vector2I(5, 5), new Vector2I(5, 6) };



		SetCellsTerrainPath(0, currentPathI, 0, 0);
		//foreach (Vector2I cell in currentPathI)
		//{

		//          //SetCell(0, new Vector2I(Mathf.FloorToInt(cell.X), Mathf.FloorToInt(cell.Y)), 0);
		//          SetCell(0, cell, 0);
		//      }
	}

	private void Stop()
	{
		_pathfinder = null;
		Clear();
	}

    //using _Ready to test this script
    public override void _Ready()
    {
        Vector2 rect_start = new Vector2(4,4);
        Vector2 rect_end = new Vector2(10,8);

		List<Vector2> points = new List<Vector2>();

		for (int x = 0; x <= Mathf.FloorToInt(rect_end.X - rect_start.X); x++)
		{
            for (int y = 0; y <= Mathf.FloorToInt(rect_end.Y - rect_start.Y); y++)
            {
				points.Add(rect_start + new Vector2(x,y));
            }
        }

		Initialize(points.ToArray());

		Vector2 path_end = new Vector2(8, 7);

		DrawPath(rect_start,path_end);
    }
}
