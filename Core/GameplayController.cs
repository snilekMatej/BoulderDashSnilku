using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.Core
{
    public class GameplayController
    {
        private const float StarterDelay = 2f;
        private const float RestartDelay = 2f;

        private GameplayState state;
        private float timer;

        public bool IsPlaying => state == GameplayState.Playing;

        public GameplayController()
        {
            StartLevel();
        }

        public void StartLevel()
        {
            state = GameplayState.WaitingToStart;
            timer = StarterDelay;
        }

        public bool Update(GameTime gameTime, Player player)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (state)
            {
                case GameplayState.WaitingToStart:
                    timer -= deltaTime;

                    if (timer <= 0f)
                    {
                        state = GameplayState.Playing;
                    }
                    break;
                case GameplayState.Playing:
                    if (!player.IsAlive)
                    {
                        state = GameplayState.WaitingToRestart;
                        timer = RestartDelay;
                    }
                    break;
                case GameplayState.WaitingToRestart:
                    timer -= deltaTime;

                    if (timer <= 0f)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }
    }
}
