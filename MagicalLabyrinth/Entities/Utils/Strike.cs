using System;
using MagicalLabyrinth.Sprites;
using Microsoft.Xna.Framework.Audio;

namespace MagicalLabyrinth.Entities.Utils;

public class Strike: IUpdate
{
    private readonly Player _player;
    private readonly AnimatedSprite _sprite;
    private readonly Action<Creature> _action;

    public bool IsNeedAction { get; private set; }
    public bool IsNeedNext { get; private set; }
    
    public bool IsStriking { get; private set; }
    private readonly SoundEffect _strikeSoundEffect = MainGame.Instance.Content.Load<SoundEffect>("sounds/sword sound");

    public Strike(Player player, AnimatedSprite sprite, Action<Creature> action)
    {
        _player = player;
        _sprite = sprite;
        _action = action;
    }

    public void Update(float dt)
    {
        if (IsNeedAction && _sprite.Progress > .33f)
        {
            MainGame.Screen.ProcessDamageZone(true, _action, _player.GetMeleeDamageZone());
            IsNeedAction = false;
        }
        
        if (IsNeedNext && _sprite.Progress > .66f)
            Start();
    }

    private void Start(string name = null)
    {
        IsNeedNext = false;
        IsNeedAction = true;
        IsStriking = true;

        name ??= _sprite.CurrentAnimationName == "strike2" ? "strike3" : "strike2";
        
        _sprite.Play(name, () => IsStriking = false, 1 - _player.Body.AbilityPack.BaseAttackSpeed);
        _strikeSoundEffect.Play();
    }

    public void TryStart()
    {
        if (IsStriking)
            IsNeedNext = true;
        else
            Start("strike1"); 
    }

    public void Cancel()
    {
        IsNeedNext = false;
        IsNeedAction = false;
        IsStriking = false;
        _sprite.Play("idle");
    }
}