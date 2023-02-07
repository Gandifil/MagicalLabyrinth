using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;

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
        
        var panel = new Panel(Anchor.Center, size: new Vector2(100, 100), positionOffset: Vector2.Zero);
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