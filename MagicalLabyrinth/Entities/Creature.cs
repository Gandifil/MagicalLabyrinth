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
        Position = new Vector2(X, CURRENT_FLOOR);

        MaxHP = _creatureData.Hp;
        HP = _creatureData.Hp;
        Level = _creatureData.Level;
    }


    public int HP { get; private set; }

    public int MaxHP { get; private set; }

    public float CURRENT_FLOOR => Y_FLOOR_LEVEL - _sprite.Origin.Y;
    
    public float TweenerColor { get; set; } = 0f;

    public void Hurt(int damage)
    {
        HP -= damage;
        if (HP <= 0)
            Die();
        TweenerColor = 1;
        MainGame.Screen.Tweener
           .TweenTo(
                target: this,
                expression: sprite => sprite.TweenerColor,
                toValue: 0f, duration: .5f, delay: 0f)
            .Easing(EasingFunctions.BounceOut);
    }

    protected int _isMoving = 0;
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        var dt = gameTime.GetElapsedSeconds();
        _position.X += _isMoving * _direction * dt * _creatureData.Speed;
    }

    public int Level { get; set; } = 1;
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Color = TweenerColor < .2f ? Color.White : Color.Red;
        base.Draw(spriteBatch);
    }

    public RectangleF GetMeleeDamageZone()
    {
        return new RectangleF(Position.X-(_direction == -1 ? _sprite.TextureRegion.Width : 0), Position.Y, 
            _sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
    }
}