using System;
using System.Numerics;
using MagicalLabyrinth.Abilities;
using MagicalLabyrinth.Entities.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;
using AnimatedSprite = MagicalLabyrinth.Sprites.AnimatedSprite;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace MagicalLabyrinth.Entities;

public class Player: Creature
{
    public int Expirience { get; private set; } = 0;

    public int MaxExpirience { get; private set; } = 8;

    public int SkillPoints { get; private set; } = 1;

    public AbilityPack AbilityPack { get; private set; } = new AbilityPack();


    public void AddExpirience(int value)
    {
        Expirience += value;
        while (Expirience >= MaxExpirience) LevelUp();
    }

    public void BuyAbility(AbilityData abilityData)
    {
        if (SkillPoints > 0 && !AbilityPack.Contain(abilityData.Name))
        {
            SkillPoints--;
            AbilityPack.AddAbility(abilityData);
        }
    }

    private void LevelUp()
    {
        Expirience -= MaxExpirience;
        MaxExpirience *= 2;
        SkillPoints++;
        Level++;
    }

    public Player(float X): base("player", X)
    {
        MainGame.Instance.KeyboardListener.KeyPressed += OnKeyPressed;
        _strike = new Strike(this, _sprite, OnBaseAttackCollised);
    }
    
    public override void Update(GameTime gameTime)
    {
        var dt = gameTime.GetElapsedSeconds();
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
            if (AbilityPack.HasTag("knifeFlow"))
                ThrowKnife();

        if (_ySpeed == 0f && !_strike.IsStriking) 
            _sprite.Play(animation);

        if (_ySpeed != 0f)
        {
            _position.Y += dt * _ySpeed;
            _ySpeed += dt * 200.8f;
            if (_position.Y > CURRENT_FLOOR)
            {
                _position.Y = CURRENT_FLOOR;
                _ySpeed = 0f;
            }
        }

        if (AbilityPack.HasTag("regeneration"))
        {
            if (_regenerationTimer.IsCompleted)
            {
                HP = Math.Min(MaxHP, HP + 10);
                _regenerationTimer.Reset(5f);
            }
            _regenerationTimer.Update(dt);
        }

        if (MaxHP == _creatureData.Hp && AbilityPack.HasTag("moreHealth"))
        {
            MaxHP *= 2;
            HP = Math.Min(MaxHP, HP + _creatureData.Hp);
        }
        
        _strike.Update(gameTime);
        _timer.Update(dt);
        base.Update(gameTime);
    }

    private float _ySpeed = 0f;
    private readonly Strike _strike;

    private void OnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.Space && _ySpeed == 0f)
        {
            _ySpeed = -200f;
            _strike.Cancel();
            _sprite.Play("jump");
        }
        
        if (e.Key == Keys.Q)
            if (!AbilityPack.HasTag("knifeFlow"))
                ThrowKnife();

        if (AbilityPack.HasTag("shift"))
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
        var action = (Creature c) => c.Hurt((int)((1 + AbilityPack.SecondAttackPower)*_creatureData.SecondAttack));

        if (AbilityPack.HasTag("knifeMultiple"))
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
        _timer.Reset(_creatureData.SecondCooldown*(1-AbilityPack.SecondCooldown));
    }

    private void OnBaseAttackCollised(Creature creature)
    {
        creature.Hurt((int)((1 + AbilityPack.BaseAttackPower)*_creatureData.BaseAttack));
    }
}