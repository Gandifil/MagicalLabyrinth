﻿using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;
using AnimatedSprite = MagicalLabyrinth.Sprites.AnimatedSprite;

namespace MagicalLabyrinth.Entities;

public class Player: Creature
{
    public Player(): base("player")
    {
        MainGame.Instance.KeyboardListener.KeyPressed += OnKeyPressed;
    }
    
    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var walkSpeed = dt * 128;
        var keyboardState = Keyboard.GetState();

        var animation = "idle";
        if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))  && !_isStriking)
        {
            _sprite.Effect = SpriteEffects.FlipHorizontally;
            _position.X -= walkSpeed;
            animation = "walk";
        }
        
        if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))  && !_isStriking)
        {
            _sprite.Effect = SpriteEffects.None;
            _position.X += walkSpeed;
            
            animation = "walk";
        }
        
        if (_ySpeed == 0f && !_isStriking) 
            _sprite.Play(animation);
        _sprite.Update(dt);

        if (_ySpeed != 0f)
        {
            _position.Y += dt * _ySpeed;
            _ySpeed += dt * 200.8f;
            if (_position.Y > XLINE)
            {
                _position.Y = XLINE;
                _ySpeed = 0f;
            }
        }
    }

    private float _ySpeed = 0f;
    private bool _isStriking = false;

    private void OnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.Space && _ySpeed == 0f)
        {
            
            _ySpeed = -200f;
            _sprite.Play("jump");
        }

        if (e.Key == Keys.LeftShift)
        {
            var keyboardState = Keyboard.GetState();
            if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))  && !_isStriking)
            {
                _sprite.Effect = SpriteEffects.FlipHorizontally;
                _position.X -= 20;
            }
        
            else if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))  && !_isStriking)
            {
                _sprite.Effect = SpriteEffects.None;
                _position.X += 20;
            }
        }
        if (e.Key == Keys.F)
        {
            _isStriking = true;        
            if (_sprite.CurrentAnimationName.StartsWith("strike")
                && _sprite.Progress is > 0.5f and < 0.95f)
            {
                _sprite.Play(_sprite.CurrentAnimationName == "strike2" ? "strike3" : "strike2",
                    () => _isStriking = false);
            }
            else
                //_sprite.Play("strike1");
            _sprite.Play("strike1", () => _isStriking = false);
        }
    }
}