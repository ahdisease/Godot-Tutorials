using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameBoard : Node2D
{
    [ExportGroup("Resources")]
    //resources
    [Export] private Grid _grid;

    //cached Nodes
    UnitOverlay _unitOverlay;
    UnitPath _unitPath;

    //state variables
    Dictionary<Vector2I,Unit> _units = new();
    Unit _activeUnit = null;
    Vector2I[] _walkableCells = null;
    bool _canIssueCommands = true;


    public override void _Ready()
	{
        _unitOverlay = GetNode<UnitOverlay>("UnitOverlay");
        _unitPath = GetNode<UnitPath>("UnitPath");
        
        _Reinitialize();
    }


    private bool IsOccupied(Vector2I position)
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

            _units.Add(unit.GetCell(),unit);
        }
    }

    public Vector2I[] GetWalkableCells(Unit unit)
    {
        return _FloodFill(unit.GetCell(), unit.MoveRange).ToArray();
    }

    /// <summary>
    /// Used to indicate valid movement cells based on movement capabilities of unit and whether or not a cell is occupied.
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="maxDistance"></param>
    /// <returns>Valid cells as a list of Vector2I</returns>
    private List<Vector2I> _FloodFill(Vector2I cell, int maxDistance)
    {
        List<Vector2I> fillRange = new();

        //create the stack of cells to check neighbors of cell for valid traversal
        Stack<Vector2I> toBeChecked = new();
        toBeChecked.Push(cell);
        
        //loop through until empty
        while (toBeChecked.Count > 0)
        {
            Vector2I currentCell = toBeChecked.Pop();

            if (!_grid.IsWithinBounds(currentCell))
            {
                continue;
            }

            if (fillRange.Contains(currentCell))
            {
                continue;
            }

            //ensure that cell is within range of unit's movement
            Vector2I differenceBetweenCells = (currentCell - cell).Abs();
            int distanceBetweenCells = differenceBetweenCells.X + differenceBetweenCells.Y;
            if (distanceBetweenCells > maxDistance)
            {
                continue;
            }

            fillRange.Add(currentCell);

            _AddNeighboringCellsToBeChecked(fillRange, toBeChecked, currentCell);
        }


        return fillRange;
    }

    /// <summary>
    /// Adds neighboring cells to the stack utilized in <seealso cref="_FloodFill(Vector2I, int)"/> if neighbors are within the grid and unoccupied.
    /// </summary>
    /// <param name="fillRange"></param>
    /// <param name="toBeChecked"></param>
    /// <param name="currentCell"></param>
    private void _AddNeighboringCellsToBeChecked(List<Vector2I> fillRange, Stack<Vector2I> toBeChecked, Vector2I currentCell)
    {
        foreach (Vector2I direction in Pathfinder.DIRECTIONS)
        {
            Vector2I neighboringCell = currentCell + direction;

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
    
    /// <summary>
    /// Sets <see cref="_activeUnit"/> and initializes relavent pathfinding nodes.
    /// </summary>
    /// <param name="cell"></param>
    public void _SelectUnit(Vector2I cell)
    {
        if (!_units.ContainsKey(cell))
        {
            return;
        }
       
        _activeUnit = _units[cell];
        _activeUnit.SetIsSelected(true);
        _walkableCells = GetWalkableCells(_activeUnit);
        _unitOverlay.DrawOverlay(_walkableCells);
        _unitPath.Initialize(_walkableCells);
    }

    /// <summary>
    /// Used to step backwards from selection state
    /// </summary>
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

    /// <summary>
    /// Used to ready gameboard for a different selection after completing previous selection action.
    /// </summary>
    public void _ClearActiveUnit()
    {
        if ( _activeUnit == null) { return; }

        _activeUnit = null;
        _walkableCells = null;
    }

    /// <summary>
    /// Determines if new cell is a valid movement and then sets Unit to new location.
    /// </summary>
    /// <param name="newCell"></param>
    public void _MoveActiveUnit(Vector2I newCell)
    {
        if(IsOccupied(newCell) || !_walkableCells.Contains(newCell))
        {
            return;
        }

        _units.Remove(_activeUnit.GetCell());
        _units[newCell] = _activeUnit;

        _DeselectUnit();

        _activeUnit.WalkAlong(_unitPath.CurrentPath);
        
        _ClearActiveUnit();
    }

    public void _ResetCanIssueCommands()
    {
        _canIssueCommands = true;
    }

    /// <summary>
    /// Listens for <seealso cref="Cursor.AcceptPressed"/> event and selects or moves unit as appropriate.
    /// </summary>
    /// <param name="cell"></param>
    public void OnCursorAcceptPressed(Vector2I cell)
    {
        
        if (_activeUnit == null)
        {
            _SelectUnit(cell);
        } else if (_activeUnit.IsSelected())
        {
            _MoveActiveUnit(cell);
        }
    }

    /// <summary>
    /// Listens for <seealso cref="Cursor.CursorMoved"/> event and draws active unit's movement path to cell inidcated by the cursor's location.
    /// </summary>
    /// <param name="cell"></param>
    public void OnCursorMoved(Vector2I cell)
    {
        if (_activeUnit != null && _activeUnit.IsSelected())
        {
            _unitPath.DrawPath(_activeUnit.GetCell(), cell);
        }
    }
}
