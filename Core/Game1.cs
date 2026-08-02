using BoulderDashSnilku.Input;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Simulation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using BoulderDashSnilku.Rendering;

namespace BoulderDashSnilku.Core;

public class Game1 : Game
{
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

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        currentLevel = 0;
        gameSession = new GameSession();

        playerLogic = new PlayerLogic();
        explosionLogic = new ExplosionLogic();

        worldSimulation = new WorldSimulation();
        _input = new InputManager();

        gameplayController = new GameplayController();
        gameState = GameState.StartScreen;

        LoadLevel(currentLevel);

        _graphics.PreferredBackBufferWidth = world.Width * TileSize * PixelScale;
        _graphics.PreferredBackBufferHeight = (world.Height * TileSize + HudHeight) * PixelScale;
        _graphics.ApplyChanges();

        base.Initialize();
    }

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

        Texture2D playerRexture = Content.Load<Texture2D>("Player/PlayerDefault");

        Texture2D fireflyTexture = Content.Load<Texture2D>("Enemies/Firefly");
        Texture2D butterflyTexture = Content.Load<Texture2D>("Enemies/Butterfly");

        worldRenderer = new WorldRenderer(dirtTexture, wallTexture, borderTexture, boulderTexture, gemTexture, exitTexture, emptyTexture);
        entityRenderer = new EntityRenderer(fireflyTexture, butterflyTexture);
        playerRenderer = new PlayerRenderer(playerRexture);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update(gameTime);
        switch (gameState)
        {
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
        if (_input.IsPressed(Keys.Escape))
        {
            Exit();
        }
        base.Update(gameTime);
    }

    private void UpdateStartScreen()
    {
        if (_input.IsPressed(Keys.Enter))
        {
            currentLevel = 0;
            gameSession = new GameSession();

            LoadLevel(currentLevel);
            gameState = GameState.Gameplay;
        }
    }

    private void UpdateEndScreen()
    {
        if (_input.IsPressed(Keys.Enter))
        {
            gameState = GameState.StartScreen;
        }
    }

    private void UpdateGameplay(GameTime gameTime)
    {
        bool restartLevel = gameplayController.Update(gameTime, player);

        if (restartLevel)
        {
            LoadLevel(currentLevel);
            return;
        }
        if (gameplayController.IsPlaying)
        {
            UpdateLevelTimer(gameTime);

            MoveDirection direction = _input.GetMoveDirection();
            playerLogic.Update(player, world, entityManager, levelState, gameSession, direction, explosionLogic);

            if (levelState.IsCompleted)
            {
                gameSession.Score += levelState.TimeLeft;

                currentLevel++;
                if (!LoadLevel(currentLevel))
                {
                    gameState = GameState.EndScreen;
                }
            }
            else
            {
                worldSimulation.Update(world, entityManager, gameTime);
            }
        }
    }

    private void UpdateLevelTimer(GameTime gameTime)
    {
        timerAccumulator += gameTime.ElapsedGameTime.TotalSeconds;

        while (timerAccumulator >= 1.0)
        {
            timerAccumulator -= 1.0;

            if (levelState.TimeLeft > 0)
            {
                levelState.TimeLeft--;
            }
            if (levelState.TimeLeft <= 0 && player.IsAlive)
            {
                player.Kill(world, entityManager, explosionLogic);
                return;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateScale(PixelScale));

        switch (gameState)
        {
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
    private void DrawStartScreen()
    {
        const string title = "BOULDER DASH";
        const string instruction = "PRESS ENTER TO START";

        float logicalScreenWidth = _graphics.PreferredBackBufferWidth / PixelScale;
        float logicalScreenHeight = _graphics.PreferredBackBufferHeight / PixelScale;

        hudFont.DrawCenteredText(_spriteBatch, title, logicalScreenHeight / 2 - 24, logicalScreenWidth, Color.White);
        hudFont.DrawCenteredText(_spriteBatch, instruction, logicalScreenHeight / 2 + 8, logicalScreenWidth, Color.White);
    }

    private void DrawGameplay()
    {
        worldRenderer.Draw(_spriteBatch, world, TileSize, HudHeight);
        playerRenderer.Draw(_spriteBatch, player, TileSize, HudHeight);
        entityRenderer.Draw(_spriteBatch, entityManager, TileSize, HudHeight);
        hudRenderer.Draw(_spriteBatch, gameSession, levelState);
    }

    private void DrawEndScreen()
    {
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

        hudFont.DrawCenteredText(_spriteBatch, title, boxY + 18, logicalWidth, Color.White);
        hudFont.DrawCenteredText(_spriteBatch, scoreText, boxY + 50, logicalWidth, Color.White);
        hudFont.DrawCenteredText(_spriteBatch, thanks, boxY + 82, logicalWidth, Color.White);
    }


    private bool LoadLevel(int levelNumber)
    {
        string fileName = $"Level{levelNumber:D2}.txt";
        string levelPath = Path.Combine(AppContext.BaseDirectory, "Content", "Levels", fileName);

        if (!File.Exists(levelPath))
        {
            return false;
        }
        LoadedLevel loadedLevel = LevelLoader.Load(levelPath);

        world = loadedLevel.World;
        worldSimulation?.Reset();
        player = loadedLevel.Player;
        levelState = loadedLevel.LevelState;
        entityManager = loadedLevel.EntityManager;

        gameplayController.StartLevel();
        return true;
    }
}
