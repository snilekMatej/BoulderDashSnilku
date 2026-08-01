using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Rendering
{
    public class WorldRenderer
    {
        private readonly Texture2D dirt;
        private readonly Texture2D wall;
        private readonly Texture2D border;
        private readonly Texture2D boulder;
        private readonly Texture2D gem;
        private readonly Texture2D exit;
        private readonly Texture2D empty;

        public WorldRenderer(Texture2D dirt, Texture2D wall, Texture2D border, Texture2D boulder, Texture2D gem, Texture2D exit, Texture2D empty)
        {
            this.dirt = dirt;
            this.wall = wall;
            this.border = border;
            this.boulder = boulder;
            this.gem = gem;
            this.exit = exit;
            this.empty = empty;
        }

        public void Draw(SpriteBatch spriteBatch, GameWorld world, int tileSize, int offsetY)
        {
            for (int y = 0; y < world.Height; y++)
            {
                for (int x = 0; x < world.Width; x++)
                {
                    Texture2D? texture = GetTexture(world.Grid[x, y]);

                    if (texture != null)
                    {
                        Rectangle destination = new Rectangle(x * tileSize, offsetY + y * tileSize, tileSize, tileSize);

                        spriteBatch.Draw(texture, destination, Color.White);
                    }
                }
            }
        }

        private Texture2D? GetTexture(Tile tile)
        {
            return tile switch
            {
                Tile.Dirt => dirt,
                Tile.Wall => wall,
                Tile.Border => border,
                Tile.Boulder => boulder,
                Tile.Gem => gem,
                Tile.Exit => exit,
                Tile.Empty => empty,

                _ => null
            };
        }
    }
}
