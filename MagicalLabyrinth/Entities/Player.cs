﻿using System;
using MagicalLabyrinth.Entities.Utils;
using MagicalLabyrinth.Mechanics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MagicalLabyrinth.Entities;

public class Player: Creature
{
    public readonly EvolvingBody Body;
    //private readonly MoveCollisions _collisions;
    
    public Player(float X): base("player", X)
    {
        _collisions = new MoveCollisions();
        MainGame.Instance.KeyboardListener.KeyPressed += OnKeyPressed;
        _strike = new Strike(this, _sprite, OnBaseAttackCollised);
        Body = new EvolvingBody(_creatureData);
        base.Body = Body;
    }
    
    public override void Update(float dt)
    {
        var keyboardState = Keyboard.GetState();

        var animation = "idle";
        _isMoving = 0;
        if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)))
        {
            SetLeftDirection();
            animation = "walk";
            _isMoving = 1;
        }
        
        if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)))
        {
            SetRightDirection();
            animation = "walk";
            _isMoving = 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Q))
            if (Body.Abilities["knifeFlow"])
                ThrowKnife();

        if (_shift.Y == 0f && !_strike.IsStriking) 
            _sprite.Play(animation);
        _shift.Y += dt * 300.8f;

        if (Body.Abilities["regeneration"])
        {
            if (_regenerationTimer.IsCompleted)
            {
                Body.HP.Value = Math.Min(Body.MaxHP, Body.HP + 10);
                _regenerationTimer.Reset(5f);
            }
            _regenerationTimer.Update(dt);
        }
        // TODO: impls ability not this way
        // if (Body.MaxHP == _creatureData.Hp && Body.AbilityPack.HasTag("moreHealth"))
        // {
        //     Body.MaxHP.Value *= 2;
        //     Body.HP.Value = Math.Min(Body.MaxHP, Body.HP + _creatureData.Hp);
        // }

        
        _strike.Update(dt);
        _timer.Update(dt);
        base.Update(dt);
    }

    private readonly Strike _strike;

    private void OnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.E)
        {
            foreach (var entity in MainGame.Screen.Entities)
                if (entity is Door door)
                    if ((Position - door.Position).Length() < 200)
                        door.Use();
        }
            
            
        if (e.Key == Keys.Space && _shift.Y == 0f)
        {
            _shift.Y = -200f;
            _strike.Cancel();
            _sprite.Play("jump");
        }
        
        if (e.Key == Keys.Q)
            if (!Body.Abilities["knifeFlow"])
                ThrowKnife();

        if (Body.Abilities["shift"])
        if (e.Key == Keys.LeftShift)
        {
            _strike.Cancel();
            var keyboardState = Keyboard.GetState();
            if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left)))
            {
                _sprite.Effect = SpriteEffects.FlipHorizontally;
                _position.X -= 20;
            }
        
            else if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right)))
            {
                _sprite.Effect = SpriteEffects.None;
                _position.X += 20;
            }
        }
        if (e.Key == Keys.F)
            _strike.TryStart();
    }

    private readonly Timer _regenerationTimer = new Timer();
    private readonly Timer _timer = new Timer();

    private readonly SoundEffect _throwSoundEffect = MainGame.Instance.Content.Load<SoundEffect>("sounds/steam");
    private void ThrowKnife()
    {
        if (!_timer.IsCompleted) return;

        var source = Position + _sprite.Origin / 2;

        var action = (Creature c) => c.Body.Hurt(new Impact(Body, (int)Body[AttributeType.SecondaryAttack]));

        if (Body.Abilities["knifeMultiple"])
        {
            MainGame.Screen.Spawn(new Projectile(this, new Vector2(_direction * 300f, -70f), action)
            {
                Position = source,
            });
            MainGame.Screen.Spawn(new Projectile(this, new Vector2(_direction * 300f, 70f), action)
            {
                Position = source,
            });
        }
        
        MainGame.Screen.Spawn(new Projectile(this, new Vector2(_direction * 300f, 0f), action)
        {
            Position = source,
        });

        _throwSoundEffect.Play(.2f, 1f, 1f);
        _timer.Reset(Body[AttributeType.SecondaryCooldown]);
    }

    private void OnBaseAttackCollised(Creature creature)
    {
        creature.Body.Hurt(new Impact(Body, (int)Body[AttributeType.MainAttack]));
    }
}