using Godot;
using System;
using System.Collections.Generic;

//RefCounted is the C# equivalent to Reference in GDScript. Because of the
//garbage collector, unreferenced items will not instantly be removed
public partial class Pathfinder : RefCounted
{

    readonly Vector2[] DIRECTIONS = new Vector2[] { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };

    private Grid _grid;
    private AStar2D _astar = new AStar2D();

    public Pathfinder(Grid grid, Vector2[] walkableCells)
    {
        //we can pass these values from the UnitPath script that we will write later
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

    //returns the path found between two cells as an array
    public Vector2[] CalculatePointPath(Vector2 start, Vector2 end)
    {

        int startIndex = _grid.AsIndex(start);
        int endIndex = _grid.AsIndex(end);

        if (_astar.HasPoint(endIndex) && _astar.HasPoint(startIndex))
        {
            Vector2[] pointPath = _astar.GetPointPath(startIndex, endIndex);
            return pointPath;
        } else
        {
            return Array.Empty<Vector2>();
        }

    }

    //Adds and connects walkable cells to Astar2D object
    private void AddAndConnectPoints(Dictionary<Vector2, int> cellMappings)
    {
        //register all points in the AStar graph
        foreach (Vector2 point in  cellMappings.Keys)
        {
            _astar.AddPoint(cellMappings[point], point);
        }
        GD.Print(_astar.GetPointCount());
        //connect points to their neighbors
        foreach (Vector2 point in cellMappings.Keys)
        {
            foreach(int neighborIndex in FindNeighborIndices(point,cellMappings))
            {
                _astar.ConnectPoints(cellMappings[point], neighborIndex);
            }
        }
        
    }

    private IEnumerable<int> FindNeighborIndices(Vector2 point, Dictionary<Vector2, int> cellMappings)
    {
        //since the number of connections is a little wiggly based on
        //weird math and where things meet, we'll return this list as
        //an array if we have to
        List<int> neighborIndices = new List<int>();

        foreach (Vector2 direction in DIRECTIONS)
        {
            //generate a potential neighbor based on vector
            Vector2 neighbor = point + direction;

            //only include walkable cells
            if (!cellMappings.ContainsKey(neighbor))
            {
                continue;
            }

            //add cell to array if not a duplicate
            if (_astar.ArePointsConnected(cellMappings[point], cellMappings[neighbor]))
            {
                neighborIndices.Add(cellMappings[neighbor]);
            }

        }

        return neighborIndices;


    }
}
