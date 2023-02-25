using System;
using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace MagicalLabyrinth.Entities;

public class Golem: Creature
{
    private readonly MainScreen _screen;
    
    public Golem(MainScreen screen, int X): base("Golem", X)
    {
        _screen = screen;
    }

    public override void Update(float dt)
    {        
        var animation = "idle";
        _isMoving = 0;
        if (NeedToLeft()  && !_isStriking)
        {
            SetLeftDirection();
            animation = "walk";
            _isMoving = 1;
        }
        
        if (NeedToRight() && !_isStriking)
        {
            SetRightDirection();
            animation = "walk";
            _isMoving = 1;
        }

        if (NeedToStrike() && !_isStriking)
            Strike();
        
        if (!_isStriking) 
            _sprite.Play(animation);
        
        
        base.Update(dt);
    }

    private readonly SoundEffect _strikeSoundEffect = MainGame.Instance.Content.Load<SoundEffect>("sounds/hit03");
    //hit03.mp3
    private void Strike(string name = "strike1")
    {
        _sprite.Play(name, () =>
        {
            _strikeSoundEffect.Play();
            MainGame.Screen.ProcessDamageZone(false, creature => creature.Hurt(30), GetMeleeDamageZone());
            _isStriking = false;
        });
        _isStriking = true;
    }

    private bool NeedToStrike()
    {
        return Math.Abs(_screen.Player.Position.X- Position.X) < _sprite.TextureRegion.Width / 2
            && Math.Abs(_screen.Player.Position.Y- Position.Y) < _sprite.TextureRegion.Height / 2;
    }

    private bool NeedToLeft()
    {
        return _screen.Player.Position.X < Position.X;
    }

    private bool NeedToRight()
    {
        return _screen.Player.Position.X > Position.X;
    }

    private bool _isStriking = false;
}