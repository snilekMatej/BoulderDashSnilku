using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.Rendering {
    /// <summary>
    /// Draws all living entities except for player.
    /// Texture is assigned based on entity type.
    /// </summary>
    public class EntityRenderer {
        private readonly Texture2D fireflyTexture;
        private readonly Texture2D butterflyTexture;

        public EntityRenderer(Texture2D fireflyTexture, Texture2D butterflyTexture) {
            this.fireflyTexture = fireflyTexture;
            this.butterflyTexture = butterflyTexture;
        }

        /// <summary>
        /// Draw all living enemies in EntityManager.
        /// </summary>
        /// <exception cref="InvalidOperationException">Missing texture for that enemy.</exception>
        public void Draw(SpriteBatch spriteBatch, EntityManager entityManager,
            int tileSize, int offsetY) {
            foreach (Enemy enemy in entityManager.GetEntities<Enemy>()) {
                if (enemy.IsAlive) {
                    Texture2D texture = enemy switch {
                        Firefly => fireflyTexture,
                        Butterfly => butterflyTexture,
                        _ => throw new InvalidOperationException(
                            $"Missing texture for enemy type {enemy.GetType().Name}.")
                    };
                    DrawEntity(spriteBatch, enemy, texture, tileSize, offsetY);
                }
            }
        }

        /// <summary>
        /// Draw one entity inside corresponding world tile.
        /// </summary>
        private static void DrawEntity(SpriteBatch spriteBatch, Entity entity,
            Texture2D texture, int tileSize, int offsetY) {
            Rectangle destination = new Rectangle(
                entity.x * tileSize, entity.y * tileSize + offsetY, tileSize, tileSize);
            spriteBatch.Draw(texture, destination, Color.White);
        }
    }
}
