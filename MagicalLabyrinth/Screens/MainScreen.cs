using System;
using System.Collections.Generic;
using System.Linq;
using MagicalLabyrinth.Entities;
using MagicalLabyrinth.Entities.Utils;
using MagicalLabyrinth.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Tweening;

namespace MagicalLabyrinth.Screens;

public class MainScreen: GameScreen
{
    private new MainGame Game => (MainGame) base.Game;

    public IEnumerable<IEntity> Entities => _entities;

    TiledMap _tiledMap;
    //MouseButton
    private TiledMapRenderer _tiledMapRenderer;
    private readonly EntitiesCollection _entities = new();

    public TiledMap TiledMap => _tiledMap;
    public Player Player { get; private set; }
    public Tweener Tweener { get; private set; } = new Tweener();

    public MainScreen(MainGame game) : base(game)
    {
    }

    private void PlayerOnDied()
    {
        ScreenManager.LoadScreen(new MessageScreen("К сожалению, вы не справились с испытанием. Рекомендуем попробовать снова.", this));
    }

    private ProgressBar _hp;
    private ProgressBar _exp;
    private SpriteFont _font;

    public override void Initialize()
    {
        base.Initialize();
        
        _tiledMap = Content.Load<TiledMap>("maps/StartLocation");
        LoadObjects();
        _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
        
        Player = new Player(250);
        _entities.Add(Player);
        Player.Body.OnDied += PlayerOnDied;
        
        _hp = new(Game.SpriteBatch)
        {
            Width = Game.Graphics.PreferredBackBufferWidth / 2,
            Height = Game.Graphics.PreferredBackBufferHeight / 30,
            BorderWidth = 6,
            BackgroundColor = Color.Transparent,
        };

        _exp = new(Game.SpriteBatch)
        {
            Width = Game.Graphics.PreferredBackBufferWidth / 4,
            Height = Game.Graphics.PreferredBackBufferHeight / 30,
            BorderWidth = 6,
            LowColor = Color.Cyan,
            HighColor = Color.Cyan,
        };
    }

    private void LoadObjects()
    {
        foreach (var objectLayer in _tiledMap.ObjectLayers)
        foreach (var obj in objectLayer.Objects)
        {
            var rect = new RectangleF(obj.Position, obj.Size);
            IEntity entity = null;
            if (obj.Name.StartsWith("redSkinEnemy"))
                entity = new RedSkinEnemy(this, (int)rect.Center.X);
            if (obj.Name.StartsWith("golem"))
                entity = new Golem(this, (int)rect.Center.X);
            if (obj.Name.StartsWith("door"))
                entity = new Door(rect.Center);
            
            if (entity != null)
                _entities.Add(entity);
        }
    }

    public override void LoadContent()
    {
        base.LoadContent();
        _font = Content.Load<SpriteFont>("fonts/gui");
        
        MainGame.Instance.KeyboardListener.KeyPressed += KeyboardListenerOnKeyPressed;
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
        
        MainGame.Instance.KeyboardListener.KeyPressed -= KeyboardListenerOnKeyPressed;
    }

    private void KeyboardListenerOnKeyPressed(object sender, KeyboardEventArgs e)
    {
        if (e.Key == Keys.B)
            MainGame.Instance.ScreenManager.LoadScreen(new AbilitiesScreen(this));
    }

    private const float CAMERA_SPEED = 128;
    private const float CAMERA_BORDER_BUFFER = 16;


    private readonly Spawner _spawner = new();
    public override void Update(GameTime gameTime)
    {
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _entities.Update(dt);
        Tweener.Update(dt);
        _tiledMapRenderer.Update(gameTime);

        if (Game.Camera.BoundingRectangle.Right < _tiledMap.WidthInPixels - CAMERA_BORDER_BUFFER)
            if ((Game.Camera.BoundingRectangle.X + Game.Camera.BoundingRectangle.Width * .75f) < Player.Position.X)
                Game.Camera.Move(new Vector2(dt * CAMERA_SPEED, 0));
        
        if (Game.Camera.BoundingRectangle.X > CAMERA_BORDER_BUFFER)
            if ((Game.Camera.BoundingRectangle.X + Game.Camera.BoundingRectangle.Width * .25f) > Player.Position.X)
                Game.Camera.Move(new Vector2(-dt * CAMERA_SPEED, 0));

        _spawner.Update(dt);
        
        if (Player.Position.X < 10) Player.Position = new Vector2(10, Player.Position.Y);
        if (Player.Position.X > _tiledMap.WidthInPixels - 10 - CAMERA_BORDER_BUFFER) 
            Player.Position = new Vector2(_tiledMap.WidthInPixels - 10  - CAMERA_BORDER_BUFFER, Player.Position.Y);
        
    }


    public override void Draw(GameTime gameTime)
    {
        _tiledMapRenderer.Draw(Game.Camera.GetViewMatrix()); 
        _entities.Draw(Game.SpriteBatch);
        
        _hp.Draw(Game.InterfaceSpriteBatch, new (0, 0), 
            (float)Player.Body.HP / Player.Body.MaxHP);
        Game.InterfaceSpriteBatch.DrawString(_font, $"HP: {Player.Body.HP}/{Player.Body.MaxHP}", 
            new Vector2(_hp.Width + 10, 0)
            , Color.Black, 0f, new Vector2(), 3f, SpriteEffects.None, 0f);

        _exp.Draw(Game.InterfaceSpriteBatch, new (0, _hp.Height + _hp.BorderWidth * 2), 
            (float)Player.Body.Expirience / Player.Body.MaxExpirience);
        Game.InterfaceSpriteBatch.DrawString(_font, $"Lvl: {Player.Body.Level}", 
            new Vector2(_exp.Width + 10, _hp.Height + _hp.BorderWidth * 2)
            , Color.Black, 0f, new Vector2(), 3f, SpriteEffects.None, 0f);
    }

    public void ProcessDamageZone(bool isFromPlayer, Action<Creature> action, RectangleF damageZone)
    {
        foreach (var entity in _entities)
            if (entity is Creature creature)
                if (isFromPlayer != entity is Player)
                    if (damageZone.Intersects(entity.HitBox))
                        action.Invoke(creature);
    }


    public int GetSpawnPoint()
    {
        var x = (float)Random.Shared.Next(0, _tiledMap.WidthInPixels);

        if (Game.Camera.BoundingRectangle.Left + CAMERA_BORDER_BUFFER < x &&
            x < Game.Camera.BoundingRectangle.Right - CAMERA_BORDER_BUFFER)
            x -= Game.Camera.BoundingRectangle.Width;
        
        if (x < 0)
            x += 2 * Game.Camera.BoundingRectangle.Width;
        return (int)x;
    }

    public void Spawn(IEntity entity)
    {
        _entities.Add(entity);
    }
}