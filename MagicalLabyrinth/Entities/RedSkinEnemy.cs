using System;
using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace MagicalLabyrinth.Entities;

public class RedSkinEnemy:Creature
{
    private readonly MainScreen _screen;
    
    public RedSkinEnemy(MainScreen screen, int X): base("adventurer-Sheet", X)
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

    private readonly SoundEffect _strikeSoundEffect = MainGame.Instance.Content.Load<SoundEffect>("sounds/melee sound");
    private void Strike(string name = "strike")
    {
        _sprite.Play(name + new Random().Next(1,4), () =>
        {
            MainGame.Screen.ProcessDamageZone(false, creature => creature.Hurt(10), GetMeleeDamageZone());
            _isStriking = false;
        });
        _isStriking = true;
        _strikeSoundEffect.Play(.2f, 1f, 1f);
    }

    private bool NeedToStrike()
    {
        return Math.Abs(_screen.Player.Position.X- Position.X) < 30
            && Math.Abs(_screen.Player.Position.Y- Position.Y) < 10;
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