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
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.Tweening;

namespace MagicalLabyrinth.Screens;

public class MainScreen: GameScreen
{
    private new MainGame Game => (MainGame) base.Game;

    private List<IEntity> _entities = new();

    public IReadOnlyList<IEntity> Entities => _entities;

    TiledMap _tiledMap;
    TiledMapRenderer _tiledMapRenderer;

    public Player Player { get; private set; }
    public Tweener Tweener { get; private set; } = new Tweener();

    public MainScreen(MainGame game) : base(game)
    {
        Player = new Player(250);
        _entities.Add(Player);
        _entities.Add(new Necromancer(this, 50));
    }

    private ProgressBar _hp;
    private ProgressBar _exp;

    public override void Initialize()
    {
        base.Initialize();
        
        _tiledMap = Content.Load<TiledMap>("maps/StartLocation");
        _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);
        
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

    public override void LoadContent()
    {
        base.LoadContent();
        
        
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
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            MainGame.Instance.Exit();

        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        foreach (var entity in _entities)
            entity.Update(gameTime);
        _entities.AddRange(_spawnEntities);
        _spawnEntities.Clear();
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
        

        var entitiesForRemove = _entities.Where(x => !x.IsAlive).ToList();
        foreach (var entity in entitiesForRemove)
        {
            if (entity is Creature creature)
                Player.AddExpirience(creature.Level);
        }

        _entities.RemoveAll(x => !x.IsAlive);
    }


    public override void Draw(GameTime gameTime)
    {
        var transformMatrix = Game.Camera.GetViewMatrix();
        _tiledMapRenderer.Draw(transformMatrix); 
        foreach (var entity in _entities)
            entity.Draw(Game.SpriteBatch);
        
        _hp.Draw(Game.InterfaceSpriteBatch, new (0, 0), 
            (float)Player.HP / Player.MaxHP);
        _exp.Draw(Game.InterfaceSpriteBatch, new (0, _hp.Height + _hp.BorderWidth * 2), 
            (float)Player.Expirience / Player.MaxExpirience);
    }

    public void ProcessDamageZone(bool isFromPlayer, Action<Creature> action, RectangleF damageZone)
    {
        foreach (var entity in _entities)
            if (entity is Creature creature)
                if (isFromPlayer != entity is Player)
                    if (damageZone.Intersects(entity.HitBox))
                        action.Invoke(creature);
    }

    private readonly List<IEntity> _spawnEntities = new();

    public void Spawn(IEntity entity)
    {
        _spawnEntities.Add(entity);
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
}