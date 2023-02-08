using MagicalLabyrinth.Abilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MLEM.Misc;
using MLEM.Ui;
using MLEM.Ui.Elements;
using MLEM.Ui.Style;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;

namespace MagicalLabyrinth.Screens;

public class MessageScreen: BackScreenBase
{
    private readonly string _message;
    
    public MessageScreen(string message, Screen backScreen) : base(backScreen)
    {
        _message = message;
    }
    

    public override void LoadContent()
    {
        base.LoadContent();

        
        var panel = new Panel(Anchor.Center, size: new Vector2(.5f, .5f), positionOffset: Vector2.Zero)
        {
            Padding = new StyleProp<Padding>(new Padding(-10))
        };
        panel.AddChild(new Paragraph(Anchor.TopCenter, 1, _message, true));
        
        panel.AddChild(new VerticalSpace(10));
        panel.AddChild(new Button(Anchor.BottomCenter, new Vector2(.5f, .1f), "Выход")
        {
            OnPressed = (element => Exit())
        });
        MainGame.Instance.UiSystem.Add("ExampleUi", panel);
        
        MainGame.Instance.KeyboardListener.KeyPressed += KeyboardListenerOnKeyPressed;
    }

    private void KeyboardListenerOnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.Escape)
            Exit();
    }

    private void Exit()
    {
        MainGame.Instance.Exit();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();

        MainGame.Instance.KeyboardListener.KeyPressed -= KeyboardListenerOnKeyPressed;

        var system = MainGame.Instance.UiSystem;
        foreach (var root in system.GetRootElements())
            system.Remove(root.Name);
    }
}