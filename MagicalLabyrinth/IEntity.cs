using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MagicalLabyrinth;

public interface IEntity: IUpdate, IMovable
{
    void Draw(SpriteBatch spriteBatch);
}