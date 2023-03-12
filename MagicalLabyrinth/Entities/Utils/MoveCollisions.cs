using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MLEM.Extended.Tiled;
using MonoGame.Extended;

namespace MagicalLabyrinth.Entities.Utils;

public class MoveCollisions
{
    private readonly TiledMapCollisions _collisions;

    public MoveCollisions()
    {
        _collisions = new TiledMapCollisions(MainGame.Screen.TiledMap);
    }

    public void CheckShift(RectangleF area, ref Vector2 dtshift, ref Vector2 shift)
    {
        area.Position += dtshift;
        area.Position /= new Vector2(32, 32);
        area.Size /= new Vector2(32, 32);
        Debug.WriteLine(area.Center.ToString());
        var collideds = _collisions.GetCollidingAreas(area);
        foreach (var collidedRect in collideds)
        {
            var direction = collidedRect.Center - area.Center;
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (dtshift.X > 0 && collidedRect.Left < area.Right)
                    dtshift.X = 0;
                if (dtshift.X < 0 && area.Left < collidedRect.Right)
                    dtshift.X = 0;
            }
            else
                if (dtshift.Y > 0 && collidedRect.Top < area.Bottom)
                {
                    dtshift.Y = Math.Max(0, dtshift.Y-32 * (area.Bottom - collidedRect.Top));
                    shift.Y = 0;
                }
        }
        
        
    }
}