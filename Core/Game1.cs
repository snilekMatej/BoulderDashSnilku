using BoulderDashSnilku.Input;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Simulation;
using BoulderDashSnilku.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;

namespace BoulderDashSnilku.Core;

public class Game1 : Game {
    private const int PixelScale = 2;
    private const int TileSize = 16;
    private const int HudHeight = 32;

    private GameWorld world;
    private int currentLevel;
    private GameSession gameSession;
    private LevelState levelState;
    private double timerAccumulator = 0;
    private EntityManager entityManager;
    private Player player;

    private PlayerLogic playerLogic;
    private ExplosionLogic explosionLogic;
    private WorldSimulation worldSimulation;
    private InputManager _input;

    private BitmapFont hudFont;
    private HudRenderer hudRenderer;
    private WorldRenderer worldRenderer;
    private EntityRenderer entityRenderer;
    private PlayerRenderer playerRenderer;

    private GameplayController gameplayController;
    private GameState gameState;

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D pixel;

    public Game1() {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    /// Creates the initial game state and gameplay systems.
    /// </summary>
    protected override void Initialize() {
        currentLevel = 0;
        gameSession = new GameSession();

        playerLogic = new PlayerLogic();
        explosionLogic = new ExplosionLogic();
        worldSimulation = new WorldSimulation();
        _input = new InputManager();

        gameplayController = new GameplayController();
        gameState = GameState.StartScreen;

        LoadLevel(currentLevel);

        _graphics.PreferredBackBufferWidth =
            world.Width * TileSize * PixelScale;
        _graphics.PreferredBackBufferHeight = 
            (world.Height * TileSize + HudHeight) * PixelScale;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    /// <summary>
    /// Loads all textures and creates renderers used by game.
    /// Content is loaded once and then reused in changes of all levels.
    /// </summary>
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D fontTexture = Content.Load<Texture2D>("Fonts/codepage437-8x8");
        hudFont = new BitmapFont(fontTexture);
        hudRenderer = new HudRenderer(hudFont);

        Texture2D dirtTexture = Content.Load<Texture2D>("Tiles/Dirt");
        Texture2D wallTexture = Content.Load<Texture2D>("Tiles/Wall");
        Texture2D borderTexture = Content.Load<Texture2D>("Tiles/Border");
        Texture2D boulderTexture = Content.Load<Texture2D>("Tiles/Boulder");
        Texture2D gemTexture = Content.Load<Texture2D>("Tiles/Gem");
        Texture2D exitTexture = Content.Load<Texture2D>("Tiles/Exit");
        Texture2D emptyTexture = Content.Load<Texture2D>("Tiles/Empty");

        Texture2D playerTexture = Content.Load<Texture2D>("Player/PlayerDefault");
        Texture2D fireflyTexture = Content.Load<Texture2D>("Enemies/Firefly");
        Texture2D butterflyTexture = Content.Load<Texture2D>("Enemies/Butterfly");

        worldRenderer = new WorldRenderer(dirtTexture, wallTexture, borderTexture,
            boulderTexture, gemTexture, exitTexture, emptyTexture);
        entityRenderer = new EntityRenderer(fireflyTexture, butterflyTexture);
        playerRenderer = new PlayerRenderer(playerTexture);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
    }

    /// <summary>
    /// Updates input and game state based on the input.
    /// Escape key can close the program at any game state.
    /// </summary>
    /// <param name="gameTime">Timing info for the current frame.</param>
    protected override void Update(GameTime gameTime) {
        _input.Update(gameTime);
        switch (gameState) {
            case GameState.StartScreen:
                UpdateStartScreen();
                break;
            case GameState.Gameplay:
                UpdateGameplay(gameTime);
                break;
            case GameState.EndScreen:
                UpdateEndScreen();
                break;
        }
        if (_input.IsPressed(Keys.Escape)) Exit();
        base.Update(gameTime);
    }

    /// <summary>
    /// Waits for player to start the game.
    /// Starting resets level numer and makes new game session.
    /// </summary>
    private void UpdateStartScreen() {
        if (_input.IsPressed(Keys.Enter)) {
            currentLevel = 0;
            gameSession = new GameSession();
            LoadLevel(currentLevel);
            gameState = GameState.Gameplay;
        }
    }

    /// <summary>
    /// Waits for player to Enter the title screen.
    /// </summary>
    private void UpdateEndScreen() {
        if (_input.IsPressed(Keys.Enter)) gameState = GameState.StartScreen;
    }

    /// <summary>
    /// Updates the current running level, restarting level, time, movement and world simulation.
    /// Completing a level awards player with remaining time score and loads the next level.
    /// Dying resets the level and Dying with no remaining lives returns to the title screen.
    /// </summary>
    /// <param name="gameTime">Timing information for current frame.</param>
    private void UpdateGameplay(GameTime gameTime) {
        bool restartLevel = gameplayController.Update(gameTime, player);
        if (restartLevel) {
            bool gameEnded = HandlePlayerDeath();
            if (!gameEnded) LoadLevel(currentLevel);
        }
        else if (gameplayController.IsPlaying) {
            UpdateLevelTimer(gameTime);
            MoveDirection direction = _input.GetMoveDirection();
            playerLogic.Update(player, world, entityManager, levelState,
                gameSession, direction, explosionLogic);
            if (levelState.IsCompleted) {
                gameSession.Score += levelState.TimeLeft;
                currentLevel++;
                bool nextLevelLoaded = LoadLevel(currentLevel);
                if (!nextLevelLoaded) gameState = GameState.EndScreen;
            }
            else worldSimulation.Update(world, entityManager, gameTime);
        }
    }

    /// <summary>
    /// Counts down the current level time in one-second intervals.
    /// When timer reaches zero: player is killed.
    /// </summary>
    /// <param name="gameTime">Timing information to measure elapsed seconds</param>
    private void UpdateLevelTimer(GameTime gameTime) {
        timerAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
        bool playerKilled = false;
        while (timerAccumulator >= 1.0 && !playerKilled) {
            timerAccumulator -= 1.0;
            if (levelState.TimeLeft > 0) levelState.TimeLeft--;
            if (levelState.TimeLeft <= 0 && player.IsAlive) {
                player.Kill(world, entityManager, explosionLogic);
                playerKilled = true;
            }
        }
    }

    /// <summary>
    /// Draws the active screen.
    /// The game is rendered according to the current game state.
    /// </summary>
    /// <param name="gameTime">Timing information for current frame</param>
    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateScale(PixelScale));
        switch (gameState) {
            case GameState.StartScreen:
                DrawStartScreen();
                break;
            case GameState.Gameplay:
                DrawGameplay();
                break;
            case GameState.EndScreen:
                DrawEndScreen();
                break;
        }
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    /// <summary>
    /// Draws the title screen.
    /// It displaies the information to start the game.
    /// </summary>
    private void DrawStartScreen() {
        const string title = "BOULDER DASH";
        const string instruction = "PRESS ENTER TO START";
        float logicalScreenWidth = _graphics.PreferredBackBufferWidth / PixelScale;
        float logicalScreenHeight = _graphics.PreferredBackBufferHeight / PixelScale;
        hudFont.DrawCenteredText(_spriteBatch, title,
            logicalScreenHeight / 2 - 24, logicalScreenWidth, Color.White);
        hudFont.DrawCenteredText(_spriteBatch, instruction,
            logicalScreenHeight / 2 + 8, logicalScreenWidth, Color.White);
    }

    /// <summary>
    /// Draws the level, player, enemies and HUD.
    /// </summary>
    private void DrawGameplay() {
        worldRenderer.Draw(_spriteBatch, world, TileSize, HudHeight);
        playerRenderer.Draw(_spriteBatch, player, TileSize, HudHeight);
        entityRenderer.Draw(_spriteBatch, entityManager, TileSize, HudHeight);
        hudRenderer.Draw(_spriteBatch, gameSession, levelState);
    }

    /// <summary>
    /// Draws the victory screen with player's final score.
    /// </summary>
    private void DrawEndScreen() {
        const int boxWidth = 260;
        const int boxHeight = 120;
        float logicalWidth = _graphics.PreferredBackBufferWidth / PixelScale;
        float logicalHeight = _graphics.PreferredBackBufferHeight / PixelScale;
        int boxX = (int)((logicalWidth - boxWidth) / 2);
        int boxY = (int)((logicalHeight - boxHeight) / 2);
        Rectangle box = new Rectangle(boxX, boxY, boxWidth, boxHeight);
        _spriteBatch.Draw(pixel, box, Color.DarkBlue);
        const string title = "YOU WIN!";
        string scoreText = $"YOUR SCORE IS {gameSession.Score:D5}";
        const string thanks = "THANKS FOR PLAYING!!!";
        hudFont.DrawCenteredText(_spriteBatch, title,
            boxY + 18, logicalWidth, Color.White);
        hudFont.DrawCenteredText(_spriteBatch, scoreText,
            boxY + 50, logicalWidth, Color.White);
        hudFont.DrawCenteredText(_spriteBatch, thanks,
            boxY + 82, logicalWidth, Color.White);
    }

    /// <summary>
    /// Loads the requested level and replaces all current level tiles.
    /// There is a pause when level is loaded.
    /// </summary>
    /// <param name="levelNumber">Number of level to load.</param>
    /// <returns>True -> level file exists and was loaded.</returns>
    private bool LoadLevel(int levelNumber) {
        string fileName = $"Level{levelNumber:D2}.txt";
        string levelPath = Path.Combine(AppContext.BaseDirectory,
            "Content", "Levels", fileName);
        bool levelLoaded = File.Exists(levelPath);
        if (levelLoaded) {
            LoadedLevel loadedLevel = LevelLoader.Load(levelPath);
            world = loadedLevel.World;
            player = loadedLevel.Player;
            levelState = loadedLevel.LevelState;
            entityManager = loadedLevel.EntityManager;
            timerAccumulator = 0;
            worldSimulation?.Reset();
            gameplayController.StartLevel();
        }
        return levelLoaded;
    }

    /// <summary>
    /// Removes one life after the player's death.
    /// If life count is below zero: Game returns to title screan.
    /// </summary>
    /// <returns>Ture -> Player has no remaining lives == game ended.</returns>
    private bool HandlePlayerDeath() {
        gameSession.PlayreLives--;
        bool gameEnded = gameSession.PlayreLives < 0;
        if (gameEnded) gameState = GameState.StartScreen;
        return gameEnded;
    }
}