using System.Numerics;
using MagicalLabyrinth.Abilities;
using MagicalLabyrinth.Entities.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    public void AddExpirience()
    {
        Expirience++;
        while (Expirience > MaxExpirience) LevelUp();
    }

    public void BuyAbility(AbilityData abilityData)
    {
        if (SkillPoints > 0)
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
    }

    public Player(float X): base("player", X)
    {
        MainGame.Instance.KeyboardListener.KeyPressed += OnKeyPressed;
    }
    
    public override void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();

        var animation = "idle";
        _isMoving = 0;
        if ((keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))  && !_isStriking)
        {
            SetLeftDirection();
            animation = "walk";
            _isMoving = 1;
        }
        
        if ((keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))  && !_isStriking)
        {
            SetRightDirection();
            animation = "walk";
            _isMoving = 1;
        }
        
        
        
        if (keyboardState.IsKeyDown(Keys.Q))
            if (AbilityPack.HasTag("knifeFlow"))
                ThrowKnife();

        if (_ySpeed == 0f && !_isStriking) 
            _sprite.Play(animation);

        if (_ySpeed != 0f)
        {
            var dt = gameTime.GetElapsedSeconds();
            _position.Y += dt * _ySpeed;
            _ySpeed += dt * 200.8f;
            if (_position.Y > CURRENT_FLOOR)
            {
                _position.Y = CURRENT_FLOOR;
                _ySpeed = 0f;
            }
        }
        _strike?.Update(gameTime);
        _timer.Update(gameTime.GetElapsedSeconds());
        base.Update(gameTime);
    }

    private float _ySpeed = 0f;
    private bool _isStriking = false;
    private Strike _strike;

    private void OnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.Space && _ySpeed == 0f)
        {
            
            _ySpeed = -200f;
            _sprite.Play("jump");
        }
        
        if (e.Key == Keys.Q)
            if (!AbilityPack.HasTag("knifeFlow"))
                ThrowKnife();

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
            _strike = new Strike(_sprite, () => MainGame.Screen.ProcessDamageZone(true, 
                (int)((1 + AbilityPack.BaseAttackPower)*_creatureData.BaseAttack), GetDamageZone()));
            if (_sprite.CurrentAnimationName.StartsWith("strike")
                && _sprite.Progress is > 0.5f and < 0.95f)
            {
                _sprite.Play(_sprite.CurrentAnimationName == "strike2" ? "strike3" : "strike2",
                    () => _isStriking = false, 1/(1 + AbilityPack.BaseAttackPower));
            }
            else
                //_sprite.Play("strike1");
            _sprite.Play("strike1", () => _isStriking = false, 1/(1 + AbilityPack.BaseAttackPower));
        }
    }

    private readonly Timer _timer = new Timer();

    private void ThrowKnife()
    {
        if (!_timer.IsCompleted) return;

        if (AbilityPack.HasTag("knifeMultiple"))
        {
            MainGame.Screen.Spawn(new Projectile(this, new Vector2(_direction * 300f, -70f))
            {
                Position = Position + _sprite.Origin / 2,
            });
            MainGame.Screen.Spawn(new Projectile(this, new Vector2(_direction * 300f, 70f))
            {
                Position = Position + _sprite.Origin / 2,
            });
        }
        
        MainGame.Screen.Spawn(new Projectile(this, new Vector2(_direction * 300f, 0f))
        {
            Position = Position + _sprite.Origin / 2,
        });
        _timer.Reset(_creatureData.SecondCooldown*(1-AbilityPack.SecondCooldown));
    }

    private RectangleF GetDamageZone()
    {
        return new RectangleF(Position.X-(_direction == -1 ? _sprite.TextureRegion.Width : 0), Position.Y, 
            _sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
    }
}