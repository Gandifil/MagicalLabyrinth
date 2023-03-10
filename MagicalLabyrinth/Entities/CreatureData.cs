using System.Collections.Generic;
using MagicalLabyrinth.Mechanics;

namespace MagicalLabyrinth.Entities;

public class CreatureData
{
    public string SpriteSheetName { get; set; }

    public IDictionary<AttributeType, float> Attributes { get; set; }
    
    public int Level { get; set; }
}

