using MagicalLabyrinth.Entities.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using AnimatedSprite = MagicalLabyrinth.Sprites.AnimatedSprite;

namespace MagicalLabyrinth.Entities;

public abstract class Entity: IEntity
{
    protected AnimatedSprite _sprite;
    protected Vector2 _shift;
    protected MoveCollisions _collisions;

    public virtual void Update(float dt)
    {
        var shift = dt * _shift;
        _collisions?.CheckShift(GetBox(), ref shift, ref _shift);
        Position += shift;
        _sprite.Update(dt);
    }

    protected void SetupAnimatedSprite(string contentName, string playAnimation = "idle")
    {
        var spriteSheet =  MainGame.Instance.Content
            .Load<SpriteSheet>(contentName, new JsonContentLoader());
        
        _sprite = new AnimatedSprite(spriteSheet, playAnimation);
    }

    public const float Y_FLOOR_LEVEL = 192;

    protected Vector2 _position;

    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, Position, rotation: _rotation);
    }

    public bool IsAlive { get; private set; } = true;
    public virtual RectangleF HitBox { get => new RectangleF((Position - _sprite.Origin).ToPoint(), _sprite.TextureRegion.Size); }

    protected void Die()
    {
        IsAlive = false;
    }

    protected int _direction = 1;
    protected float _rotation;

    protected void SetLeftDirection()
    {
        _sprite.Effect = SpriteEffects.FlipHorizontally;
        _direction = -1;
    }

    protected void SetRightDirection()
    {
        _sprite.Effect = SpriteEffects.None;
        _direction = 1;
    }

    public RectangleF GetMeleeDamageZone()
    {
        return new RectangleF(Position.X-(_direction == -1 ? _sprite.TextureRegion.Width : 0), Position.Y, 
            _sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
    }

    public RectangleF GetBox()
    {
        return _sprite.GetBoundingRectangle(Position, 0f, Vector2.One);
    }
}