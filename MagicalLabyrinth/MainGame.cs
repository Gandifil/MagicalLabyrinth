using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;

namespace MagicalLabyrinth;

public class MainGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public SpriteBatch SpriteBatch => _spriteBatch;
    
    private readonly ScreenManager _screenManager;
    
    private readonly TouchListener _touchListener;
    private readonly GamePadListener _gamePadListener;
    public readonly KeyboardListener KeyboardListener;
    private readonly MouseListener _mouseListener;
    
    public OrthographicCamera Camera { get; private set; }

    public static MainGame Instance;
    public static MainScreen Screen;

    public MainGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        //_graphics.IsFullScreen = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _screenManager = new ScreenManager();
        Components.Add(_screenManager);
        
        KeyboardListener = new KeyboardListener();
        _gamePadListener = new GamePadListener();
        _mouseListener = new MouseListener();
        _touchListener = new TouchListener();

        Instance = this;
        Components.Add(new InputListenerComponent(this, KeyboardListener, _gamePadListener, _mouseListener, _touchListener));
    }

    protected override void Initialize()
    {
        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 400, 240);
        Camera = new OrthographicCamera(viewportAdapter);

        Screen = new MainScreen(this);
        _screenManager.LoadScreen(Screen, new FadeTransition(GraphicsDevice, Color.Black));
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        var transformMatrix = Camera.GetViewMatrix();
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);
        base.Draw(gameTime);
        SpriteBatch.End();
    }
}