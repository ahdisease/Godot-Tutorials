using Godot;
using System;
using System.Collections.Generic;

//RefCounted is the C# equivalent to Reference in GDScript. Because of the
//garbage collector, unreferenced instances will not instantly be removed
public partial class Pathfinder : RefCounted
{

    readonly Vector2[] DIRECTIONS = new Vector2[] { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };

    private Grid _grid;
    private AStar2D _astar = new AStar2D();

    public Pathfinder(Grid grid, Vector2[] walkableCells)
    {
        //we can pass these values from the UnitPath script
        _grid = grid;

        //we define a dictionary with cell 2D coordinates as the key and 1D as the value
        //this makes sense from our usage standpoint (we'll be converting from 2D to 1D) but feels opposite
        //to what I've been taught (a single unique value seems a better key)
        Dictionary<Vector2,int> cellMappings = new Dictionary<Vector2,int>();

        foreach (Vector2 cell in walkableCells)
        {
            cellMappings.Add(cell, _grid.AsIndex(cell));
        }
        
        AddAndConnectPoints(cellMappings);
    }

    /// <summary>
    /// Returns the path found between two cells by the instanced AStar2D object as an array of Vector2 objects.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Vector2[] CalculatePointPath(Vector2 start, Vector2 end)
    {
        //first we confirm both points are valid grid coordinates
        int startIndex = _grid.AsIndex(start);
        int endIndex = _grid.AsIndex(end);

        if (_astar.HasPoint(endIndex) && _astar.HasPoint(startIndex))
        {
            //if so, we find the path between them
            return _astar.GetPointPath((long)startIndex, (long)endIndex); ;
        } else
        {
            //otherwise we send back an empty vector (though we could also throw an exception)
            return Array.Empty<Vector2>();
        }

    }

    /// <summary>
    /// Adds traversable cells to Astar2D object and connects neighboring cells to allow pathfinding 
    /// </summary>
    /// <param name="cellMappings"></param>
    private void AddAndConnectPoints(Dictionary<Vector2, int> cellMappings)
    {
        //register all points in the AStar graph
        foreach (Vector2 point in  cellMappings.Keys)
        {
            _astar.AddPoint(cellMappings[point], point);
        }
        
        //connect points to their neighbors
        foreach (Vector2 point in cellMappings.Keys)
        {

            List<int> neighborIndices = FindNeighborIndices(point, cellMappings);

            foreach (int neighborIndex in neighborIndices)
            {
                _astar.ConnectPoints(cellMappings[point], neighborIndex);
            }
        }
        
    }

    /// <summary>
    /// <para>Identifies the neighboring cells in a grid and adds valid cells to a List in the form of a 1D array index.</para>
    /// <para>Because this is intended for use in initializing the AStar2D object, connections that have already been added to the object are skipped.</para>
    /// </summary>
    /// <param name="point"></param>
    /// <param name="cellMappings"></param>
    private List<int> FindNeighborIndices(Vector2 point, Dictionary<Vector2, int> cellMappings)
    {
        //since the number of connections is a little wiggly based on
        //weird math and where things meet, we'll use a list
        List<int> neighborIndices = new List<int>();

        foreach (Vector2 direction in DIRECTIONS)
        {
            //generate a potential neighbor based on directional vector
            Vector2 neighbor = point + direction;

            //only include walkable cells
            if (!cellMappings.ContainsKey(neighbor))
            {
                continue;
            }

            //add cell to list if not a duplicate
            if (!_astar.ArePointsConnected(cellMappings[point], cellMappings[neighbor]))
            {
                neighborIndices.Add(cellMappings[neighbor]);
            }

        }

        return neighborIndices;
    }
}
