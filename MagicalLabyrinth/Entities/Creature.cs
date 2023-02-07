using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tweening;
using AnimatedSprite = MagicalLabyrinth.Sprites.AnimatedSprite;

namespace MagicalLabyrinth.Entities;

public abstract class Creature: IEntity
{
    protected readonly CreatureData _creatureData;
    protected AnimatedSprite _sprite;

    protected Creature(string modelName)
    {
        _creatureData = MainGame.Instance.Content
            .Load<CreatureData>("creatures/" + modelName + ".json", new JsonContentLoader());
        
        var spriteSheet =  MainGame.Instance.Content
            .Load<SpriteSheet>("creatures/" + _creatureData.SpriteSheetName, new JsonContentLoader());
        
        _sprite = new AnimatedSprite(spriteSheet, "idle");
    }

    private int _hp = 100;
    public bool IsAlive => _hp > 0;

    private Tweener _tweener = new Tweener();
    public float TweenerColor { get; set; } = 0f;

    public void Hurt(int damage)
    {
        _hp -= damage;
        _tweener
            .TweenTo(
                target: this,
                expression: sprite => sprite.TweenerColor,
                toValue: 1f, duration: .2f, delay: 0f)
            //.RepeatForever(repeatDelay: 0.1f)
            .AutoReverse()
            //.Repeat(2);
        .Easing(EasingFunctions.BounceOut);
    }

    protected int _direction = 1;
    protected int _isMoving = 0;

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
    
    public virtual void Update(GameTime gameTime)
    {
        var dt = gameTime.GetElapsedSeconds();
        _position.X += _isMoving * _direction * dt * _creatureData.Speed;
        _sprite.Update(dt);
        _tweener.Update(dt);
    }
    
    protected const float XLINE = 178;

    protected Vector2 _position = new Vector2(50, XLINE);

    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Color = TweenerColor < .8f ? Color.White : Color.Red;
        spriteBatch.Draw(_sprite, Position);
    }
}