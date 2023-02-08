using System.Collections.Generic;
using MagicalLabyrinth.Entities;
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
    
    TiledMap _tiledMap;
    TiledMapRenderer _tiledMapRenderer;

    public Player Player { get; private set; }
    public Tweener Tweener { get; private set; } = new Tweener();

    public MainScreen(MainGame game) : base(game)
    {
        Player = new Player(50);
        _entities.Add(Player);
        _entities.Add(new RedSkinEnemy(this, 250));
    }

    public override void Initialize()
    {
        base.Initialize();
        
        _tiledMap = Content.Load<TiledMap>("maps/StartLocation");
        _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

        _exp = new(Game.SpriteBatch)
        {
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

    public override void Update(GameTime gameTime)
    {
        var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        foreach (var entity in _entities)
            entity.Update(gameTime);
        _entities.AddRange(_spawnEntities);
        _spawnEntities.Clear();
        Tweener.Update(deltaSeconds);
        _tiledMapRenderer.Update(gameTime);
    }

    //private readonly ProgressBar _hp = new();
    private ProgressBar _exp;

    public override void Draw(GameTime gameTime)
    {
        var transformMatrix = Game.Camera.GetViewMatrix();
        _tiledMapRenderer.Draw(transformMatrix); 
        foreach (var entity in _entities)
            entity.Draw(Game.SpriteBatch);
        
        _exp.Draw(Game.SpriteBatch, new (0, 0), 
            (float)Player.Expirience / Player.MaxExpirience);

        _entities.RemoveAll(x => !x.IsAlive);
    }

    public void ProcessDamageZone(bool isFromPlayer, int value, RectangleF damageZone)
    {
        foreach (var entity in _entities)
            if (entity is Creature creature)
                if (isFromPlayer != entity is Player)
                {
                    if (damageZone.Intersects(entity.HitBox))
                    {
                        creature.Hurt(value);
                        Player.AddExpirience();
                    }
                    
                }
    }

    private readonly List<IEntity> _spawnEntities = new();

    public void Spawn(IEntity entity)
    {
        _spawnEntities.Add(entity);
    }
}