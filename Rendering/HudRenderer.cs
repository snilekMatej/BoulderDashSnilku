using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using BoulderDashSnilku.Core;
using BoulderDashSnilku.World;
using System.Threading.Tasks.Sources;

namespace BoulderDashSnilku.Rendering
{
    public class HudRenderer
    {
        private readonly BitmapFont font;

        public HudRenderer(BitmapFont font)
        {
            this.font = font;
        }

        public void Draw(SpriteBatch spriteBatch, GameSession gameSession, LevelState levelState)
        {
            string text = $"{levelState.RequiredGems:D2}/" + $"{levelState.GemValue:D2}   " + $"{levelState.CollectedGems:D2}   " + $"SCORE: {gameSession.Score:D5}   " + $"TIME: {levelState.TimeLeft:D3}";
            font.DrawText(spriteBatch, text, new Vector2(8, 4), Color.White);
        }
    }
}
