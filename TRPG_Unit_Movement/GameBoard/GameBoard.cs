using Godot;
using System;
using System.Collections.Generic;

public partial class GameBoard : Node2D
{
    //constants
    readonly Vector2[] DIRECTIONS = new Vector2[] { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };

    //resources
    [Export] private Grid _grid;

    //state variables
    Dictionary<Vector2,Unit> _units = new Dictionary<Vector2,Unit>();

    public override void _Ready()
	{
        _Reinitialize();
	}

    private bool IsOccupied(Vector2 position)
    {
        return _units.ContainsKey(position);
    }

    private void _Reinitialize() 
    {
        _units.Clear();

        //for this tutorial we loop through nodes, but in a larger game it would probably be beneficial to cache units or something
        foreach (Node node in GetChildren()) 
        {
            //cast as Unit. If the casting fails, it wasn't a unit.
            Unit unit = node as Unit;
            if(unit == null)
            {
                continue;
            }

            _units.Add(unit.Cell,unit);
        }
    }
}
