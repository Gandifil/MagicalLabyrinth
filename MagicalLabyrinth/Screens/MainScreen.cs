using System.Collections.Generic;
using MagicalLabyrinth.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MagicalLabyrinth.Screens;

public class MainScreen: GameScreen
{
    private new MainGame Game => (MainGame) base.Game;

    private List<IEntity> _entities = new();
    
    TiledMap _tiledMap;
    TiledMapRenderer _tiledMapRenderer;

    public Player Player { get; private set; }
    
    public MainScreen(MainGame game) : base(game) { }

    public override void LoadContent()
    {
        base.LoadContent();
        
        _tiledMap = Content.Load<TiledMap>("maps/StartLocation");
        _tiledMapRenderer = new TiledMapRenderer(GraphicsDevice, _tiledMap);

        Player = new Player();
        _entities.Add(Player);
        _entities.Add(new RedSkinEnemy(this, 250));
    }

    public override void Update(GameTime gameTime)
    {
        var deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();
        //
        // if (keyboardState.IsKeyDown(Keys.R))
        //     Game.Camera.ZoomIn(deltaSeconds);
        //
        // if (keyboardState.IsKeyDown(Keys.F))
        //     Game.Camera.ZoomOut(deltaSeconds);

        foreach (var entity in _entities)
            entity.Update(gameTime);
        _tiledMapRenderer.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        var transformMatrix = Game.Camera.GetViewMatrix();
        _tiledMapRenderer.Draw(transformMatrix); // Game.Camera.GetViewMatrix()
        foreach (var entity in _entities)
            entity.Draw(Game.SpriteBatch);

        _entities.RemoveAll(x => !x.IsAlive);
    }

    public void ProcessDamageZone(bool isFromPlayer, int value, RectangleF damageZone)
    {
        foreach (var entity in _entities)
            if (entity is Creature creature)
                if (isFromPlayer != entity is Player)
                    if (damageZone.Contains(entity.Position))
                        creature.Hurt(value);
    }
}