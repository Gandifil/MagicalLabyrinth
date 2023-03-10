using MagicalLabyrinth.Mechanics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.Tweening;

namespace MagicalLabyrinth.Entities;

public abstract class Creature: Entity
{
    protected readonly CreatureData _creatureData;

    public Body Body { get; protected set; }  

    protected Creature(string modelName, float X)
    {
        _creatureData = MainGame.Instance.Content
            .Load<CreatureData>("creatures/" + modelName + ".json", new JsonContentLoader());
        Body = new Body(_creatureData);
        Body.OnDied += Die;
        Body.OnDamaged += OnDamaged;
        SetupAnimatedSprite("creatures/" + _creatureData.SpriteSheetName);
        Position = new Vector2(X, CURRENT_FLOOR);
    }

    private void OnDamaged()
    {
        TweenerColor = 1;
        MainGame.Screen.Tweener
            .TweenTo(
                target: this,
                expression: sprite => sprite.TweenerColor,
                toValue: 0f, duration: .5f, delay: 0f)
            .Easing(EasingFunctions.BounceOut);
    }

    public float CURRENT_FLOOR => Y_FLOOR_LEVEL - _sprite.Origin.Y;
    
    public float TweenerColor { get; set; } = 0f;


    protected int _isMoving = 0;
    
    public override void Update(float dt)
    {
        base.Update(dt);
        
        _position.X += _isMoving * _direction * dt * Body[AttributeType.Speed];
    }

    
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