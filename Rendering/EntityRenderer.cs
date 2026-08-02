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
    public class EntityRenderer
    {
        private readonly Texture2D fireflyTexture;
        private readonly Texture2D butterflyTexture;

        public EntityRenderer(Texture2D fireflyTexture, Texture2D butterflyTexture)
        {
            this.fireflyTexture = fireflyTexture;
            this.butterflyTexture = butterflyTexture;
        }

        public void Draw(SpriteBatch spriteBatch, EntityManager entityManager, int tileSize, int offsetY)
        {
            foreach (Enemy enemy in entityManager.GetEntities<Enemy>())
            {
                if (enemy.IsAlive)
                {
                    Texture2D texture = enemy switch
                    {
                        Firefly => fireflyTexture,
                        Butterfly => butterflyTexture,

                        _ => throw new InvalidOperationException($"Missing texture for enemy type {enemy.GetType().Name}.")
                    };
                    DrawEntity(spriteBatch, enemy, texture, tileSize, offsetY);
                }
            }
        }

        private static void DrawEntity(SpriteBatch spriteBatch, Entity entity, Texture2D texture, int tileSize, int offsetY)
        {
            Rectangle destination = new Rectangle(entity.x * tileSize, entity.y * tileSize + offsetY, tileSize, tileSize);
            
            spriteBatch.Draw(texture, destination, Color.White);
        }
    }
}
