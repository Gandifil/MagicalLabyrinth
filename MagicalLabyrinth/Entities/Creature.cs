using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tweening;
using AnimatedSprite = MagicalLabyrinth.Sprites.AnimatedSprite;

namespace MagicalLabyrinth.Entities;

public abstract class Creature: Entity
{
    protected readonly CreatureData _creatureData;

    protected Creature(string modelName, float X)
    {
        _creatureData = MainGame.Instance.Content
            .Load<CreatureData>("creatures/" + modelName + ".json", new JsonContentLoader());
        SetupAnimatedSprite("creatures/" + _creatureData.SpriteSheetName);
        Position = new Vector2(X, XLINE);
    }

    private int _hp = 100;

    private Tweener _tweener = new Tweener();
    public float TweenerColor { get; set; } = 0f;

    public void Hurt(int damage)
    {
        _hp -= damage;
        if (_hp <= 0)
            Die();
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

    protected int _isMoving = 0;
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        var dt = gameTime.GetElapsedSeconds();
        _position.X += _isMoving * _direction * dt * _creatureData.Speed;
        _tweener.Update(dt);
    }

    public int Level { get; set; } = 1;
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Color = TweenerColor < .8f ? Color.White : Color.Red;
        base.Draw(spriteBatch);
    }
}