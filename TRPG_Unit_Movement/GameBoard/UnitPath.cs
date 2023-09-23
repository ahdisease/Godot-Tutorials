using Godot;

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
		
		currentPath = _pathfinder.CalculatePointPath(cellStart,cellEnd);
		
		foreach (Vector2 cell in currentPath)
		{
			SetCell(0,new Vector2I(Mathf.FloorToInt(cell.X), Mathf.FloorToInt(cell.Y)));
				//^SetCellv  should exist, but it's not being seen. This will do for now?
			
			//UpdateBitmaskRegion
			//this method is also not appearing. huh.
			//Haxe is not the same as Godot...BOO
			//I should refactor Vector2 into Vector2I, at least for cell values, when this is said and done
		}
	}

	private void Stop()
	{
		_pathfinder = null;
		Clear();
	}
}
