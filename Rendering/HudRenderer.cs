using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoulderDashSnilku.Core;
using BoulderDashSnilku.World;

namespace BoulderDashSnilku.Rendering {
    /// <summary>
    /// Builds and draws the gameplay HUD containing player and level information.
    /// </summary>
    public class HudRenderer {
        private readonly BitmapFont font;

        public HudRenderer(BitmapFont font) {
            this.font = font;
        }

        /// <summary>
        /// Draw the current player and level information
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, GameSession gameSession,
            LevelState levelState) {
            string text = 
                $"♥{gameSession.PlayreLives}   " +
                $"{levelState.RequiredGems:D2}/" +
                $"{levelState.GemValue:D2}   " +
                $"{levelState.CollectedGems:D2}   " +
                $"SCORE: {gameSession.Score:D5}   " +
                $"TIME: {levelState.TimeLeft:D3}";
            font.DrawText(spriteBatch, text, new Vector2(16, 8), Color.White);
        }
    }
}
