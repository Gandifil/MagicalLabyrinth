using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace MagicalLabyrinth.Tiled;

public class ParallaxTiledMapRenderer: TiledMapRenderer
{
    private const string PARALLAX_FACTOR_X_NAME = "parallaxFactorX";
    private const string PARALLAX_FACTOR_Y_NAME = "parallaxFactorY";
    
    // TODO: this class has a readonly instance of TiledMap, but base class can reload it.
    private readonly TiledMap _tiledMap;
    private readonly OrthographicCamera _camera;
    private Dictionary<int, Vector2> _parallaxFactors = new();

    public ParallaxTiledMapRenderer(OrthographicCamera camera, GraphicsDevice graphicsDevice, TiledMap map = null) : base(graphicsDevice, map)
    {
        _tiledMap = map;
        _camera = camera;

        LoadParallaxFactors();
    }

    private void LoadParallaxFactors()
    {
        _parallaxFactors.Clear();
        for (int layerIndex = 0; layerIndex < _tiledMap.Layers.Count; ++layerIndex)
            _parallaxFactors[layerIndex] = GetParallaxFactorFor(_tiledMap.Layers[layerIndex]);
    }

    private Vector2 GetParallaxFactorFor(TiledMapLayer layer)
    {
        var parallaxFactor = Vector2.One;
        if (layer.Properties.TryGetValue(PARALLAX_FACTOR_X_NAME, out string x))
            parallaxFactor.X = float.Parse(x);
        if (layer.Properties.TryGetValue(PARALLAX_FACTOR_Y_NAME, out string y))
            parallaxFactor.Y = float.Parse(y);
        return parallaxFactor;
    }

    public void Draw(
        Matrix? projectionMatrix = null,
        Effect effect = null,
        float depth = 0.0f)
    {
        for (int layerIndex = 0; layerIndex < _tiledMap.Layers.Count; ++layerIndex)
            Draw(layerIndex, _camera.GetViewMatrix(_parallaxFactors[layerIndex]), projectionMatrix, effect, depth);
    }
}