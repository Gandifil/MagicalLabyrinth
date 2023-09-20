using System;
using Microsoft.Xna.Framework;

namespace MagicalLabyrinth.Entities;

public class Door: Entity
{
    private bool _isOpen;
    
    public bool IsOpen
    {
        get => _isOpen;
        set
        {
            _isOpen = value;
            _sprite.Play(value ? "opened" : "closed");
        }
    }
    
    public Door(Vector2 position)
    {
        Position = position;
        SetupAnimatedSprite($"buildings/door1.sf", "opened");
        IsOpen = false;
    }

    public void Use()
    {
        if (IsOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        _sprite.Play("open");
    }

    public void Close()
    {
        _sprite.Play("close");
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (_sprite.CurrentAnimationName == "open" && Math.Abs(_sprite.Progress - 1f) < 0.4f)
            IsOpen = true;

        if (_sprite.CurrentAnimationName == "close" && Math.Abs(_sprite.Progress - 1f) < 0.4f)
            IsOpen = false;
    }
}