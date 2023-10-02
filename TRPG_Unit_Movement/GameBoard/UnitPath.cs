using Godot;
using System.Collections.Generic;

public partial class UnitPath : TileMap
{
    [ExportCategory("Resources")]
    //resources
    [Export] private Grid grid;

	//state variables
	private Pathfinder _pathfinder;
	public Vector2I[] CurrentPath { get; private set; }		//cached from pathfinder, used for WalkAlong()

	public void Initialize(Vector2I[] walkableCells)
	{
		_pathfinder = new Pathfinder(grid,walkableCells);
	}

	/// <summary>
	/// Generates a tilemap image of the currently selected path between two cells. 
	/// </summary>
	/// <param name="cellStart"></param>
	/// <param name="cellEnd"></param>
	public void DrawPath(Vector2I cellStart, Vector2I cellEnd)
	{
        //remove previous values on the tilemap
        Clear();    

		//determine the best path between two points
		CurrentPath = _pathfinder.CalculatePointPath(cellStart, cellEnd);

		//create Godot Array to hold Vector2I objects
		//this is necessary because SetCellsTerrainPath has no overload to accept Vector2I[] 
		Godot.Collections.Array<Vector2I> currentPathI = new();
		foreach (Vector2I cell in CurrentPath )
		{
			currentPathI.Add(cell);
		}

		//this method sets tiles at the vectors returned from the CalculatePointPath method.
		//it utilizes the first terrain set and first terrain index for the tileset in the Tilemap node
		SetCellsTerrainPath(0, currentPathI, 0, 0);

        /*see this link for the official Godot tutorial on how to implement a terrain generated Tilemap:
         * Creating terrain sets: https://docs.godotengine.org/en/latest/tutorials/2d/using_tilesets.html#doc-using-tilesets-creating-terrain-sets
         * Utilizing terrain sets in a TileMap: https://docs.godotengine.org/en/latest/tutorials/2d/using_tilemaps.html#handling-tile-connections-automatically-using-terrains
		*/

        /*
		 * It's possible that the following code could be used to avoid use of the Godot.Collections.Array, but
		 * it would most likely require a caching of the TileSetAtlas. Since I'm still rather new to Godot and there's 
		 * a pretty good terrain tutorial that makes no mention of an atlas, the above is simpler, if still less satisfying
		 *
		
		foreach (Vector2 cell in currentPath)
		{

			SetCell(0, new Vector2I(Mathf.FloorToInt(cell.X), Mathf.FloorToInt(cell.Y)), 0);
		}
		*/
    }

    /// <summary>
    /// Clears any tilemap path drawing and removes reference to current Pathfinder object to allow for garbage collection
    /// </summary>
    public void Stop()
	{
		_pathfinder = null;
		Clear();
	}

}
