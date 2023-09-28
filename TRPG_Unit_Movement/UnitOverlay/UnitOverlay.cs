using Godot;
using System;

public partial class UnitOverlay : TileMap
{
    public void DrawOverlay(Vector2I[] cells)
    {
        Clear();
  
        foreach (var cell in cells) 
        {
            SetCell(0,cell,0,Vector2I.Zero);
        }
    }
}
