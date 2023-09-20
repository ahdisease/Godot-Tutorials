using Godot;
using System;

public partial class Unit : Path2D
{
    [Export] public Grid grid;
    [Export] public int moveRange;
    [Export] public Texture2D Skin { set; get; }
    [Export] public Vector2 SkinOffset { set; get; } = Vector2.Zero;
    [Export] public float moveSpeed = 600.00f;

    public Vector2 Cell { private set; get; } = Vector2.Zero;
    public bool IsSelected { private set; get; } = false;
    public bool IsWalking { private set; get; } = false;

    //cached nodes
    private Sprite2D _sprite;
    private AnimationPlayer _anim_player;
    private PathFollow2D _path_follow;

    //signals
    [Signal] public delegate void WalkFinishedEventHandler();

    public override void _Ready()
    {
        CacheNodes();
        grid = ResourceLoader.Load<Grid>("res://Grid.tres");
        SetSkin(Skin);

        //set process false
        SetProcess(false);

        //initialize cell property by finding the intended cell based on initial position, and correcting that position to fit that cell
        Cell = grid.CalculateGridCoordinates(Position);
        Position = grid.CalculateMapPosition(Cell);

        //create the curve
        if (!Engine.IsEditorHint())
        {
            Curve = new Curve2D();
        }

        //this code is for testing purposes only
        Vector2[] testPath = new Vector2[] {new Vector2(2,2), new Vector2(2, 5), new Vector2(8, 5), new Vector2(8, 7)};
        WalkAlong(testPath);
    }

    public override void _Process(double delta)
    {
        //follow path
        _path_follow.HOffset += moveSpeed * (float)delta;

        //when the end of the path is reached, set IsWalking to off.
        //This is done using the PathFollow2D node property "offset"

        //when offset reaches 1,
        if (_path_follow.HOffset >= 1)
        {
            SetIsWalking(false);

            //reset properties for future path following
            _path_follow.HOffset = 0f;
            Position = grid.CalculateMapPosition(Cell);
            Curve.ClearPoints();

            EmitSignal("WalkFinished");
        }
    }

    public void WalkAlong(Vector2[] path)
    {
        //no effect if path is empty
        if (path == null || path.Length == 0) return;

        //add 0 as start of curve
        Curve.AddPoint(Vector2.Zero);

        //for each vector in path, add to curve as a point
        //make sure you calculate the desired position
        foreach (Vector2 point in path)
        {
            Curve.AddPoint(grid.CalculateMapPosition(point)-Position);
        }
        //set unit's cell using the new position
        setCell(path[path.Length - 1]);

        //set IsWalking to true
        SetIsWalking(true);
    }

    //Descriptive functions
    private void CacheNodes()
    {
        _sprite = GetNode<Sprite2D>("PathFollow2D/Sprite");
        _anim_player = GetNode<AnimationPlayer>("AnimationPlayer");
        _path_follow = GetNode<PathFollow2D>("PathFollow2D");
    }

    //specialized setters
    public void setCell(Vector2 value)
    {
        Cell = grid.Clamp(value);
    }

    public void SetIsSelected(bool value)
    {
        IsSelected = value;
        if (IsSelected)
        {
            _anim_player.Play("selected");
        } else
        {
            _anim_player.Play("idle");
        }
    }

    public void SetSkin(Texture2D value)
    {
        Skin = value;
        //so in GDScript the setters are called before ready.
        //I've added this because I'm pretty sure that particular loop is still relevant.
        //That said, not sure how to emulate the "yeild" at this time
        if (_sprite != null)
        {
            _sprite.Texture = value;
        }
    }

    public void SetSkinOffset(Vector2 value)
    {
        SkinOffset = value;
        if (_sprite != null)
        {
            _sprite.Position = value;
        }
    }

    public void SetIsWalking(bool value) 
    { 
        IsWalking = value;

        SetProcess(IsWalking);
    }

}
