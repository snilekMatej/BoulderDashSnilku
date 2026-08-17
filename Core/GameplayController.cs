using Microsoft.Xna.Framework;
using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.Core {
    public class GameplayController {
        private const float StarterDelay = 2f;
        private const float RestartDelay = 2f;
        private GameplayState state;
        private float timer;
        public bool IsPlaying => state == GameplayState.Playing;

        /// <summary>
        /// Creates the Gameplay controller and starts thelevel with initial level-start delay.
        /// </summary>
        public GameplayController() {
            StartLevel();
        }

        /// <summary>
        /// Resets the gameplay state before level starts / restarts.
        /// Gameplay is frozen for StarterDelay seconds before activating.
        /// </summary>
        public void StartLevel() {
            state = GameplayState.WaitingToStart;
            timer = StarterDelay;
        }

        /// <summary>
        /// Updates the current gameplay state based on elapsed time and player status.
        /// </summary>
        /// <param name="gameTime">Timing information for current frame.</param>
        /// <param name="player">Player whose IsAlive state determines level restarting.</param>
        /// <returns>True -> current level should be restarted.</returns>
        public bool Update(GameTime gameTime, Player player) {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bool restartLevel = false;
            switch (state) {
                case GameplayState.WaitingToStart:
                    timer -= deltaTime;
                    if (timer <= 0f) state = GameplayState.Playing;
                    break;
                case GameplayState.Playing:
                    if (!player.IsAlive) {
                        state = GameplayState.WaitingToRestart;
                        timer = RestartDelay;
                    }
                    break;
                case GameplayState.WaitingToRestart:
                    timer -= deltaTime;
                    restartLevel = timer <= 0f;
                    break;
            }
            return restartLevel;
        }
    }
}