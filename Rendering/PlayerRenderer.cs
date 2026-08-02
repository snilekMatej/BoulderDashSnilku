using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.Rendering
{
    public class PlayerRenderer
    {
        private readonly Texture2D playerTexture;

        public PlayerRenderer(Texture2D playerTexture)
        {
            this.playerTexture = playerTexture;
        }

        public void Draw(SpriteBatch spriteBatch, Player player, int tileSize, int offsetY)
        {
            if (player.IsAlive)
            {
                Rectangle destination = new Rectangle(player.x * tileSize, player.y * tileSize + offsetY, tileSize, tileSize);

                spriteBatch.Draw(playerTexture, destination, Color.White);
            }
        }
    }
}
