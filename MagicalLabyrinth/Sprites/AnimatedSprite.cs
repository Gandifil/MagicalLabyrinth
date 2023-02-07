using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MagicalLabyrinth.Sprites;

public class AnimatedSprite : Sprite, IAnimationController
{
    private readonly SpriteSheet _spriteSheet;
    private SpriteSheetAnimation _currentAnimation;

    public AnimatedSprite(SpriteSheet spriteSheet, string playAnimation = null)
        : base(spriteSheet.TextureAtlas[0])
    {
        _spriteSheet = spriteSheet;
        if (playAnimation == null)
            return;
        Play(playAnimation);
    }

    public SpriteSheetAnimation Play(string name, Action onCompleted = null, float speed = 1f)
    {
        if (_currentAnimation == null || _currentAnimation.IsComplete || _currentAnimation.Name != name)
        {
            SpriteSheetAnimationCycle cycle = _spriteSheet.Cycles[name];
            TextureRegion2D[] array = cycle.Frames.Select(f => _spriteSheet.TextureAtlas[f.Index]).ToArray();
            _currentAnimation = new SpriteSheetAnimation(name, array, speed * cycle.FrameDuration, cycle.IsLooping, 
                cycle.IsReversed, cycle.IsPingPong);
            if (_currentAnimation != null)
                _currentAnimation.OnCompleted = onCompleted;
        }
        return _currentAnimation;
    }

    public void Update(float deltaTime)
    {
        if (_currentAnimation == null || _currentAnimation.IsComplete)
            return;
        _currentAnimation.Update(deltaTime);
        TextureRegion = _currentAnimation.CurrentFrame;
    }

    public void Update(GameTime gameTime) => Update(gameTime.GetElapsedSeconds());

    public string CurrentAnimationName => (_currentAnimation?.IsPlaying ?? false ? _currentAnimation?.Name : null) 
                                          ?? string.Empty;

    public float Progress => (float?)_currentAnimation?.CurrentFrameIndex / _currentAnimation?.KeyFrames.Length ?? 0;

}