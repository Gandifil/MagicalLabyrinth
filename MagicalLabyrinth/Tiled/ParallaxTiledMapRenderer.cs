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
    
    public ParallaxTiledMapRenderer(OrthographicCamera camera, GraphicsDevice graphicsDevice, TiledMap map = null) : base(graphicsDevice, map)
    {
        _tiledMap = map;
        _camera = camera;
    }

    public void Draw(
        Matrix? projectionMatrix = null,
        Effect effect = null,
        float depth = 0.0f)
    {
        var defaultMatrix = _camera.GetViewMatrix();
        for (int layerIndex = 0; layerIndex < _tiledMap.Layers.Count; ++layerIndex)
        {
            var layer = _tiledMap.Layers[layerIndex];
            var viewMatrix = defaultMatrix;
            if (layer.Properties.TryGetValue(PARALLAX_FACTOR_X_NAME, out string x)
                && layer.Properties.TryGetValue(PARALLAX_FACTOR_Y_NAME, out string y))
            {
                viewMatrix = _camera.GetViewMatrix(new Vector2(float.Parse(x), float.Parse(y)));
            }
            Draw(layerIndex, viewMatrix, projectionMatrix, effect, depth);
        }
    }
}