using MagicalLabyrinth.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MLEM.Font;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Style;
using MonoGame.Extended;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;

namespace MagicalLabyrinth;

public class MainGame : Game
{
    public GraphicsDeviceManager Graphics { get; private set; }

    public SpriteBatch SpriteBatch { get; private set; }
    public SpriteBatch InterfaceSpriteBatch { get; private set; }
    public ScreenManager ScreenManager { get; private set; }
    
    private readonly TouchListener _touchListener;
    private readonly GamePadListener _gamePadListener;
    public readonly KeyboardListener KeyboardListener;
    public UiSystem UiSystem { get; private set; }
    private readonly MouseListener _mouseListener;
    
    public OrthographicCamera Camera { get; private set; }

    public static MainGame Instance;
    public static MainScreen Screen;

    public MainGame()
    {
        Graphics = new GraphicsDeviceManager(this);
        //_graphics.IsFullScreen = true;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        ScreenManager = new ScreenManager();
        Components.Add(ScreenManager);
        
        KeyboardListener = new KeyboardListener();
        _gamePadListener = new GamePadListener();
        _mouseListener = new MouseListener();
        _touchListener = new TouchListener();
        
        UiSystem = new UiSystem(this, new UiStyle());

        Instance = this;
        Components.Add(new InputListenerComponent(this, KeyboardListener, _gamePadListener, _mouseListener, _touchListener));
    }

    protected override void Initialize()
    {
        Graphics.IsFullScreen = false;
        Graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width - 50;
        Graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height - 50;
        Graphics.ApplyChanges();
            
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        InterfaceSpriteBatch = new SpriteBatch(GraphicsDevice);
        
        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 400, 240);
        Camera = new OrthographicCamera(viewportAdapter);

        Screen = new MainScreen(this);
        ScreenManager.LoadScreen(Screen, new FadeTransition(GraphicsDevice, Color.Black));
        base.Initialize();
    }

    protected override void LoadContent()
    {
        var testTexture = Content.Load<Texture2D>("Test");
        var testPatch = new NinePatch(new TextureRegion(testTexture, 40, 0, 40, 40), 16);

        var style = new UntexturedStyle(this.SpriteBatch) {
            Font = new GenericSpriteFont(
                Content.Load<SpriteFont>("fonts/gui"), 
                Content.Load<SpriteFont>("fonts/Text"), 
                Content.Load<SpriteFont>("fonts/Text")),
            PanelTexture = testPatch,
            ButtonTexture = new NinePatch(new TextureRegion(testTexture, 24, 8, 16, 16), 4),
            TextFieldTexture = new NinePatch(new TextureRegion(testTexture, 24, 8, 16, 16), 4),
            ScrollBarBackground = new NinePatch(new TextureRegion(testTexture, 12, 0, 4, 8), 1, 1, 2, 2),
            ScrollBarScrollerTexture = new NinePatch(new TextureRegion(testTexture, 8, 0, 4, 8), 1, 1, 2, 2),
            CheckboxTexture = new NinePatch(new TextureRegion(testTexture, 24, 8, 16, 16), 4),
            CheckboxCheckmark = new TextureRegion(testTexture, 24, 0, 8, 8),
            RadioTexture = new NinePatch(new TextureRegion(testTexture, 16, 0, 8, 8), 3),
            RadioCheckmark = new TextureRegion(testTexture, 32, 0, 8, 8),
            TextColor = Color.Black,
        };
        UiSystem.Style = style;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        UiSystem.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        var transformMatrix = Camera.GetViewMatrix();
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);
        InterfaceSpriteBatch.Begin();
        base.Draw(gameTime);
        InterfaceSpriteBatch.End();
        SpriteBatch.End();
        
        UiSystem.Draw(gameTime, SpriteBatch);
    }
}