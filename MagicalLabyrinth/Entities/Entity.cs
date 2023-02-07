using MagicalLabyrinth.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using AnimatedSprite = MagicalLabyrinth.Sprites.AnimatedSprite;

namespace MagicalLabyrinth.Entities;

public abstract class Entity: IEntity
{
    protected AnimatedSprite _sprite;

    public virtual void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
    }

    protected void SetupAnimatedSprite(string contentName)
    {
        var spriteSheet =  MainGame.Instance.Content
            .Load<SpriteSheet>(contentName, new JsonContentLoader());
        
        _sprite = new AnimatedSprite(spriteSheet, "idle");
    }

    public const float XLINE = 178;

    protected Vector2 _position;

    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, Position);
    }

    public bool IsAlive { get; private set; } = true;

    protected void Die()
    {
        IsAlive = false;
    }

    protected int _direction = 1;
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
}