using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Extended.Extensions;
using MLEM.Misc;
using MLEM.Ui.Style;
using MonoGame.Extended.TextureAtlases;

namespace MagicalLabyrinth.Abilities;

public class AbilityIcon: Image
{
    private readonly AbilityData _abilityData;
    private readonly Tooltip _tooltip;
    
    public AbilityIcon(Anchor anchor, AbilityData data) 
        : base(anchor, new Vector2(32, 32), getTexture(data), false)
    {
        _abilityData = data;
        CanBeMoused = true;
        CanBePressed= true;
        
        Padding = new StyleProp<Padding>(new Padding(2));
        var _tooltip = new Tooltip(data.Description, this);
        _tooltip.MouseOffset = new Vector2(32, -64);
        _tooltip.ParagraphWidth = new StyleProp<float>(300);
        CheckState();
        OnPressed += _ =>
        {
            MainGame.Screen.Player.BuyAbility(data);
            CheckState();
        };
    }

    private static TextureRegion getTexture(AbilityData abilityData)
    {
        var texture = MainGame.Instance.Content.Load<Texture2D>("abilities/"+abilityData.SpriteSheet);
        //var csize = new Point(32, 32);
        //var pos = new Point(abilityData.SpriteX, abilityData.SpriteY);
        return new TextureRegion2D(texture).ToMlem();
        
    }

    private void CheckState()
    {
        Color = MainGame.Screen.Player.AbilityPack.Contain(_abilityData.Name)
            ? new StyleProp<Color>(Microsoft.Xna.Framework.Color.White)
            : new StyleProp<Color>(Microsoft.Xna.Framework.Color.Gray);
    }
}