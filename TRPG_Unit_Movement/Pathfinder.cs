using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// A <see cref="RefCounted"/> inheriting object used to generate a path in a grid using the <see cref="AStar2D"/> object
/// </summary>
public partial class Pathfinder : RefCounted
{
    //RefCounted is the C# equivalent to Reference in GDScript. Because of the
    //garbage collector, unreferenced instances will not instantly be removed

    public static readonly Vector2I[] DIRECTIONS = new Vector2I[] { Vector2I.Left, Vector2I.Right, Vector2I.Up, Vector2I.Down };

    private Grid _grid;
    private AStar2D _astar = new AStar2D();

    public Pathfinder(Grid grid, Vector2I[] walkableCells)
    {
        //we can pass these values from the UnitPath script
        _grid = grid;

        //we define a dictionary with cell 2D coordinates as the key and 1D as the value
        Dictionary<Vector2I,long> cellMappings = new Dictionary<Vector2I,long>();

        foreach (Vector2I cell in walkableCells)
        {
            cellMappings.Add(cell, _grid.AsIndex(cell));
        }
        
        AddAndConnectPoints(cellMappings);
    }

    /// <summary>
    /// Returns the path found between two cells by the instanced AStar2D object as an array of Vector2I objects.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Vector2I[] CalculatePointPath(Vector2I start, Vector2I end)
    {
        //first we confirm both points are valid grid coordinates
        long startIndex = _grid.AsIndex(start);
        long endIndex = _grid.AsIndex(end);

        if (_astar.HasPoint(endIndex) && _astar.HasPoint(startIndex))
        {
            Vector2[] path2 = _astar.GetPointPath(startIndex, endIndex);
            Vector2I[] path2I = new Vector2I[path2.Length];

            for (int i = 0; i < path2.Length; i++)
            {
                path2I[i] = new Vector2I(Mathf.FloorToInt(path2[i].X), Mathf.FloorToInt(path2[i].Y));
            }
            //if so, we find the path between them (but we need to convert to Vector2I first)
            return path2I;
        } else
        {
            //otherwise we send back an empty vector (though we could also throw an exception)
            return Array.Empty<Vector2I>();
        }

    }

    /// <summary>
    /// Adds traversable cells to Astar2D object and connects neighboring cells to allow pathfinding 
    /// </summary>
    /// <param name="cellMappings"></param>
    private void AddAndConnectPoints(Dictionary<Vector2I, long> cellMappings)
    {
        //register all points in the AStar graph
        foreach (Vector2I point in  cellMappings.Keys)
        {
            _astar.AddPoint(cellMappings[point], point);
        }
        
        //connect points to their neighbors
        foreach (Vector2I point in cellMappings.Keys)
        {

            List<long> neighborIndices = FindNeighborIndices(point, cellMappings);

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
    private List<long> FindNeighborIndices(Vector2I point, Dictionary<Vector2I, long> cellMappings)
    {
        //since the number of connections is a little wiggly based on
        //weird math and where things meet, we'll use a list
        List<long> neighborIndices = new List<long>();

        foreach (Vector2I direction in DIRECTIONS)
        {
            //generate a potential neighbor based on directional vector
            Vector2I neighbor = point + direction;

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
