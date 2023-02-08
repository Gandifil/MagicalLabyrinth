using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace MagicalLabyrinth.Screens;

public abstract class BackScreenBase : Screen
{
    protected readonly Screen _backScreen;

    protected BackScreenBase(Screen backScreen)
    {
        _backScreen = backScreen;
    }

    protected void Back()
    {
        ScreenManager.LoadScreen(_backScreen);
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        _backScreen.Draw(gameTime);
    }
}