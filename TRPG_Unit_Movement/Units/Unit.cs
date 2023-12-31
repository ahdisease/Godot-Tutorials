using Godot;
using System;
using System.Threading.Tasks;

public partial class Unit : Path2D
{
    [ExportGroup("Resources")]
    // Resources
    [Export] private Grid grid;

    [ExportGroup("Properties")]
    //Properties

    [ExportSubgroup("Movement")]
    [Export] public int MoveRange { private set; get; }
    [Export] private float moveSpeedInPixelsPerSecond = 400f;

    [ExportSubgroup("Appearance")]
    [Export] private Texture2D Skin { set; get; }
    [Export] private Vector2I SkinOffset { set; get; } = Vector2I.Zero;
    

    // State variables
    private Vector2I cell = Vector2I.Zero;
    private bool isSelected = false;
    private bool isWalking = false;

    // Cached nodes
    private Sprite2D _sprite;
    private AnimationPlayer _anim_player;
    private PathFollow2D _path_follow;

    // Signals
    [Signal] public delegate void WalkFinishedEventHandler();

    public override void _Ready()
    {
        CacheNodes();
        grid = ResourceLoader.Load<Grid>("res://Grid.tres");
        SetSkin(Skin);
        _anim_player.Play("idle");

        //Process is only set to true when walking from point to point
        SetProcess(false);

        //initialize cell property by finding the intended cell based on initial position, and correcting that position to fit that cell
        cell = grid.CalculateGridCoordinates(Position);
        Position = grid.CalculateMapPosition(cell);

        //create the curve
        if (!Engine.IsEditorHint())
        {
            Curve = new Curve2D();
        }
    }
    /// <summary>
    /// Moves unit's sprite along a path to simulate movement until it reaches the current Cell.
    /// </summary>
    /// <param name="delta"></param>
    public override void _Process(double delta)
    {
        //note: the tutorial suggests using "offset",
        //but that property seems to have been replaced by
        //"Progress" and "ProgressRatio" properties since writing

        //follow path
        _path_follow.Progress += moveSpeedInPixelsPerSecond * (float)delta;

        //when the end of the path is reached, set IsWalking to off.
        //when offset reaches 1,
        if (_path_follow.ProgressRatio >= 1)
        {
            SetIsWalking(false);

            //reset properties for future path following
            _path_follow.Progress = 0f;
            Position = grid.CalculateMapPosition(cell);
            Curve.ClearPoints();

            EmitSignal("WalkFinished");
        }
    }

    /// <summary>
    /// <para>Used to build a path using the Path2D nodes for the Pathfollow2D node to use. Also changes the Cell property location.</para>
    /// <para>See <seealso cref="Unit._Process(double)"/> for implementation of the created path.</para>
    /// </summary>
    /// <param name="path"></param>
    public void WalkAlong(Vector2I[] path)
    {
        //no effect if path is empty
        if (path == null || path.Length == 0) return;

        //for each vector in path, add to curve as a point
        //make sure you calculate the desired position
        foreach (Vector2I point in path)
        {
            Curve.AddPoint(grid.CalculateMapPosition(point)-Position);
        }
        //set unit's cell using the new position
        SetCell(path[path.Length - 1]);

        //set IsWalking to true
        SetIsWalking(true);
        return;
    }

    // Descriptive functions
    private void CacheNodes()
    {
        _sprite = GetNode<Sprite2D>("PathFollow2D/Sprite");
        _anim_player = GetNode<AnimationPlayer>("AnimationPlayer");
        _path_follow = GetNode<PathFollow2D>("PathFollow2D");
    }

    // Getters
    public Vector2I GetCell()
    {
        return cell;
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    // Setters
    public void SetCell(Vector2I value)
    {
        cell = grid.Clamp(value);
    }

    public void SetIsSelected(bool value)
    {
        isSelected = value;
        if (isSelected)
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
            //_sprite.Position = new Vector2(this.Position.X + SkinOffset.X, this.Position.Y + SkinOffset.Y);
        }
    }

    public void SetSkinOffset(Vector2I value)
    {
        SkinOffset = value;
        if (_sprite != null)
        {
            _sprite.Position = value;
        }
    }

    public void SetIsWalking(bool value) 
    { 
        isWalking = value;

        SetProcess(isWalking);
    }

}
