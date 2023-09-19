using Godot;
using System;

public partial class Unit : Path2D
{
    [Export]
    public Grid grid = ResourceLoader.Load<Grid>("res://Grid.tres","Grid");
    [Export]
    public int moveRange;
    [Export] public Texture Skin { set; get; }
    [Export] public Vector2 SkinOffset { set; get; } = Vector2.Zero;
    [Export] public float moveSpeed = 600.00f;

    public Vector2 Cell { set; get; } = Vector2.Zero;
    public bool IsSelected { set; get; } = false;
    public bool IsWalking { set; get; } = false;

    //cached nodes
    private Sprite2D _sprite;
    private AnimationPlayer _anim_player;
    private PathFollow2D _path_follow;

    public override void _Ready()
    {
        
        CacheNodes();
    }

    private void CacheNodes()
    {
        _sprite = GetNode<Sprite2D>("Sprite");
        _anim_player = GetNode<AnimationPlayer>("AnimationPlayer");
        _path_follow = GetNode<PathFollow2D>("PathFollow2D");
    }

    public void setCell(Vector2 value)
    {
        Cell = grid.Clamp(value);
    }
}
