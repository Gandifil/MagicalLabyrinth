using MagicalLabyrinth.Abilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Content;
using MonoGame.Extended.Serialization;

namespace MagicalLabyrinth.Screens;

public class AbilitiesScreen: BackScreenBase
{
    public AbilitiesScreen(Screen backScreen) : base(backScreen)
    {
    }

    public override void LoadContent()
    {
        base.LoadContent();

        MainGame.Instance.KeyboardListener.KeyPressed += KeyboardListenerOnKeyPressed;
        
        var panel = new Panel(Anchor.Center, size: new Vector2(.5f, .5f), positionOffset: Vector2.Zero)
        {
            Padding = new StyleProp<Padding>(new Padding(-10))
        };
        panel.AddChild(new Paragraph(Anchor.TopCenter, 1, 
            "Очки способностей: " + MainGame.Screen.Player.SkillPoints, true));
        
        panel.AddChild(new VerticalSpace(10));
        
        var abilities = MainGame.Instance.Content.Load<AbilityData[]>("abilities.json", new JsonContentLoader());
        foreach (var abilityData in abilities)
            panel.AddChild(new AbilityIcon(Anchor.AutoInline, abilityData));
        panel.AddChild(new Paragraph(Anchor.BottomCenter, 1, "Esc/B - возврат к игре", true));
        MainGame.Instance.UiSystem.Add("ExampleUi", panel);
    }

    private void KeyboardListenerOnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.Escape || e.Key == Keys.B)
            Back();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();

        MainGame.Instance.KeyboardListener.KeyPressed -= KeyboardListenerOnKeyPressed;

        var system = MainGame.Instance.UiSystem;
        foreach (var root in system.GetRootElements())
            system.Remove(root.Name);
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        _backScreen.Draw(gameTime);
    }
}