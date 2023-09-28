using Godot;
using System;

/*
 * Player-controlled cursor. Allows player navigate and interact with the game grid
 * Supports keyboard/mouse/touch input
 */


[Tool]		//Tool tag allows us to use the script in the editor
public partial class Cursor : Node2D
{
	//resources
	[Export] private Grid grid;

	//properties
	[Export] private float uiCooldown = 0.1f;

	//state variables
	private Vector2I cell = Vector2I.Zero;

	//cached nodes
	Timer _timer;

	//signals
	[Signal] public delegate void AcceptPressedEventHandler(Vector2I cell);
	[Signal] public delegate void CursorMovedEventHandler(Vector2I newCell);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//initialize instance variables
        grid = ResourceLoader.Load<Grid>("res://Grid.tres");
		_timer = GetNode<Timer>("Timer");

		//initialize state
		_timer.WaitTime = uiCooldown;
		Position = grid.CalculateMapPosition(cell);
    }

	public override void _UnhandledInput(InputEvent _event) {

		if (_event.GetType() == typeof(InputEventMouseMotion))
		{
			//set cell to closest grid cell if mouse is moved
			//since I'm new to the "Get" method, the try block felt safer
			var eventPosition = _event.Get("position");
			Vector2 eventPositionVector;
			try
			{
				eventPositionVector = (Vector2)eventPosition;
			} catch (InvalidCastException exception)
			{
				GD.Print(exception);
				return;
			}

			SetCell(grid.CalculateGridCoordinates(eventPositionVector));
		} else if (_event.IsActionPressed("click") || _event.IsActionPressed("ui_accept"))
		{
			//for interactions, emit a signal and close the event
			EmitSignal(SignalName.AcceptPressed, cell);
			GetViewport().SetInputAsHandled();
		}

		//this variable stores several checks to determine if the cursor should move
		bool shouldMove = _event.IsPressed();	//should only move if a button was pressed

		if (_event.IsEcho())					//should only move if new input or timer has elapsed to prevent too many inputs accepted
		{
			shouldMove = shouldMove && _timer.IsStopped();
		}

		//end method if conditions not met
		if (!shouldMove) { return; }

		//parse directional input
		if (_event.IsAction("ui_right"))
		{
			SetCell(this.cell + Vector2I.Right);
		}
		else if (_event.IsAction("ui_up"))
		{
            SetCell(this.cell + Vector2I.Up);
        } 
		else if (_event.IsAction("ui_left"))
        {
            SetCell(this.cell + Vector2I.Left);
        } 
		else if (_event.IsAction("ui_down"))
        {
            SetCell(this.cell + Vector2I.Down);
        }
    }

	//draws two pixel outline around current cell
	private void _draw()
	{
		DrawRect(
			new Rect2(-grid.cellSize / 2, grid.cellSize), 
			new Color("aliceblue"),
			false,
			2f);
	}

	//getters
	public Vector2I GetCell() { return cell; }

	//setters
	public void SetCell(Vector2 newCell)
	{
		//confirm new cell is in range
		Vector2I clampedNewCell = grid.Clamp(newCell);		
		
		//no change required if cells match
		if (clampedNewCell.Equals(cell))
		{
			return;	
		}

		cell = clampedNewCell;

		//update sprite position
		Position = grid.CalculateMapPosition(cell);

		EmitSignal(SignalName.CursorMoved, cell);

		//reset rate limiting timer
		_timer.Start();

	}
}
