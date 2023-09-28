using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameBoard : Node2D
{
    //constants
    readonly Vector2[] DIRECTIONS = new Vector2[] { Vector2.Left, Vector2.Right, Vector2.Up, Vector2.Down };

    //resources
    [Export] private Grid _grid;

    //cached Nodes
    UnitOverlay _unitOverlay;
    UnitPath _unitPath;

    //state variables
    Dictionary<Vector2,Unit> _units = new Dictionary<Vector2,Unit>();
    Unit _activeUnit = null;
    Vector2[] _walkableCells = null;
    bool _canIssueCommands = true;


    public override void _Ready()
	{
        _unitOverlay = GetNode<UnitOverlay>("UnitOverlay");
        _unitPath = GetNode<UnitPath>("UnitPath");
        
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

    public Vector2[] GetWalkableCells(Unit unit)
    {
        return _FloodFill(unit.Cell, unit.moveRange).ToArray();
    }

    private List<Vector2> _FloodFill(Vector2 cell, int maxDistance)
    {
        List<Vector2> fillRange = new List<Vector2>();

        //create the stack of cells to check neighbors of for valid traversal
        Stack<Vector2> toBeChecked = new Stack<Vector2>();
        toBeChecked.Push(cell);
        
        //loop through until empty
        while (toBeChecked.Count > 0)
        {
            Vector2 currentCell = toBeChecked.Pop();

            if (!_grid.IsWithinBounds(currentCell))
            {
                continue;
            }

            if (fillRange.Contains(currentCell))
            {
                continue;
            }

            //ensure that cell is within range of unit's movement
            Vector2 differenceBetweenCells = (currentCell - cell).Abs();
            int distanceBetweenCells = Mathf.FloorToInt(differenceBetweenCells.X + differenceBetweenCells.Y);
            if (distanceBetweenCells > maxDistance)
            {
                continue;
            }

            fillRange.Add(currentCell);

            _AddNeighboringCellsToBeChecked(fillRange, toBeChecked, currentCell);
        }


        return fillRange;
    }

    private void _AddNeighboringCellsToBeChecked(List<Vector2> fillRange, Stack<Vector2> toBeChecked, Vector2 currentCell)
    {
        foreach (Vector2 direction in DIRECTIONS)
        {
            Vector2 neighboringCell = currentCell + direction;

            if (IsOccupied(neighboringCell))
            {
                continue;
            }

            if (fillRange.Contains(neighboringCell))
            {
                continue;
            }

            toBeChecked.Push(neighboringCell);
        }
    }
    
    public void _SelectUnit(Vector2 cell)
    {
   
        Unit unit;
        try
        {
            unit = _units[cell];
        }
        catch (KeyNotFoundException)
        {
            return;
        }
       
        _activeUnit = unit;
        _activeUnit.SetIsSelected(true);
        _walkableCells = GetWalkableCells(_activeUnit);
        _unitOverlay.Draw(_walkableCells);
        _unitPath.Initialize(_walkableCells);
    }

    public void _DeselectUnit()
    {
        if (_activeUnit == null)
        {
            return;
        }

        _activeUnit.SetIsSelected(false);
        _unitOverlay.Clear();
        _unitPath.Stop();
    }

    public void _ClearActiveUnit()
    {
        if ( _activeUnit == null) { return; }

        _activeUnit = null;
        _walkableCells = null;
    }

    public void _MoveActiveUnit(Vector2 newCell)
    {
        if(IsOccupied(newCell) || !_walkableCells.Contains(newCell))
        {
            return;
        }

        _units.Remove(_activeUnit.Cell);
        _units[newCell] = _activeUnit;

        _DeselectUnit();

        _activeUnit.WalkAlong(_unitPath.CurrentPath);
        
        _ClearActiveUnit();
    }

    public void _ResetCanIssueCommands()
    {
        _canIssueCommands = true;
    }

    public void OnCursorAcceptPressed(Vector2 cell)
    {
        
        if (_activeUnit == null)
        {
            _SelectUnit(cell);
        } else if (_activeUnit.IsSelected)
        {
            _MoveActiveUnit(cell);
        }
    }

    public void OnCursorMoved(Vector2 cell)
    {
        if (_activeUnit != null && _activeUnit.IsSelected)
        {
            _unitPath.DrawPath(_activeUnit.Cell, cell);
        }
    }
}
