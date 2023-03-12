using System;
using MagicalLabyrinth.Entities;

namespace MagicalLabyrinth.Mechanics;

public class Body
{
    protected readonly CreatureData СreatureData;

    public Body(CreatureData сreatureData)
    {
        СreatureData = сreatureData;
        
        MaxHP = new ObservedParameter<int>((int)сreatureData.Attributes[AttributeType.Hp]);
        HP = new ObservedParameter<int>(MaxHP);
        Level = сreatureData.Level;
    }
    
    public readonly ObservedParameter<int> HP;
    
    public readonly ObservedParameter<int> MaxHP;

    public virtual float this[AttributeType index] => СreatureData.Attributes[index];

    public int Level { get; protected set; } = 1;
    
    public void Hurt(IImpact impact)
    {
        HP.Value = Math.Max(0, HP - impact.Damage);
        OnDamaged?.Invoke();
        if (HP <= 0)
        {
            OnDied?.Invoke();
            (impact.Sender as EvolvingBody)?.AddExpirience(Level);
        }
    }

    public event Action OnDied;

    public event Action OnDamaged;
}