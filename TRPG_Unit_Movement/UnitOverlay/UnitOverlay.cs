using Godot;
using System;

public partial class UnitOverlay : TileMap
{
    public void Draw(Vector2[] cells)
    {
        Clear();
  
        foreach (var cell in cells) 
        {
            SetCell(0,new Vector2I(Mathf.FloorToInt(cell.X), Mathf.FloorToInt(cell.Y)),0,Vector2I.Zero);
        }
    }
}
