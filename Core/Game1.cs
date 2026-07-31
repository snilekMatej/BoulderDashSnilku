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

    private SpriteFont hudFont;
    private HudRenderer hudRenderer;

    private GameplayController gameplayController;

    private GameState gameState;
    private KeyboardState previousKeyboardState;

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

        hudFont = Content.Load<SpriteFont>("HudFont");
        hudRenderer = new HudRenderer(hudFont);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState currentKeyboardState = Keyboard.GetState();

        if (gameState == GameState.StartScreen)
        {
            bool enterPressed = currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter);

            if (enterPressed)
            {
                currentLevel = 0;
                gameSession = new GameSession();

                LoadLevel(currentLevel);
                gameState = GameState.Gameplay;
            }
            previousKeyboardState = currentKeyboardState;

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            base.Update(gameTime);
            return;
        }
        if (gameState == GameState.EndScreen)
        {
            bool enterPressed = currentKeyboardState.IsKeyDown(Keys.Enter) && previousKeyboardState.IsKeyUp(Keys.Enter);

            if (enterPressed)
            {
                gameState = GameState.StartScreen;
            }
            previousKeyboardState = currentKeyboardState;

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            base.Update(gameTime);
            return;
        }

        _input.Update(gameTime);

        bool restartLevel = gameplayController.Update(gameTime, player);

        if (restartLevel)
        {
            LoadLevel(currentLevel);
            gameplayController.StartLevel();

            base.Update(gameTime);
            return;
        }
        if (gameplayController.IsPlaying)
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
                    break;
                }
            }

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

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        previousKeyboardState = currentKeyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateScale(PixelScale));

        if (gameState == GameState.StartScreen)
        {
            DrawStartScreen();

            _spriteBatch.End();
            base.Draw(gameTime);
            return;
        }
        if (gameState == GameState.EndScreen)
        {
            DrawEndScreen();

            _spriteBatch.End();
            base.Draw(gameTime);
            return;
        }

        // Draw World
        for (int y = 0; y < world.Height; y++)
        {
            for (int x = 0; x < world.Width; x++)
            {
                Color color = Color.Green;

                switch (world.Grid[x, y])
                {
                    case Tile.Dirt: color = Color.SaddleBrown; break;
                    case Tile.Wall: color = Color.Gray; break;
                    case Tile.Border: color = Color.DarkBlue; break;
                    case Tile.Boulder: color = Color.DarkSlateGray; break;
                    case Tile.Gem: color = Color.Yellow; break;
                    case Tile.Empty: color = Color.Black; break;
                }

                _spriteBatch.Draw(pixel, new Rectangle(x * TileSize, y * TileSize + HudHeight, TileSize, TileSize), color);
            }
        }
        // Draw PLayer
        _spriteBatch.Draw(pixel, new Rectangle(player.x * TileSize, player.y * TileSize + HudHeight, TileSize, TileSize), player.IsAlive ? Color.Cyan : Color.Red);
        foreach (Firefly firefly in entityManager.GetEntities<Firefly>())
        {
            if (!firefly.IsAlive)
            {
                continue;
            }
            _spriteBatch.Draw(pixel, new Rectangle(firefly.x * TileSize, firefly.y * TileSize + HudHeight, TileSize, TileSize), Color.Purple);
        }
        foreach (Butterfly butterfly in entityManager.GetEntities<Butterfly>())
        {
            if (!butterfly.IsAlive)
            {
                continue;
            }
            _spriteBatch.Draw(pixel, new Rectangle(butterfly.x * TileSize, butterfly.y * TileSize + HudHeight, TileSize, TileSize), Color.Orange);
        }

        hudRenderer.Draw(_spriteBatch, gameSession, levelState);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
    private void DrawStartScreen()
    {
        const string title = "BOULDER     DASH";
        const string instruction = "PRESS     ENTER     TO     START";

        Vector2 titleSise = hudFont.MeasureString(title);
        Vector2 instructionSize = hudFont.MeasureString(instruction);

        float logicalScreenWidth = _graphics.PreferredBackBufferWidth / PixelScale;
        float logicalScreenHeight = _graphics.PreferredBackBufferHeight / PixelScale;

        Vector2 titlePosition = new Vector2((logicalScreenWidth - titleSise.X) / 2, logicalScreenHeight / 2 - 24);
        Vector2 instructionPosition = new Vector2((logicalScreenWidth - instructionSize.X) / 2, logicalScreenHeight / 2 + 8);

        _spriteBatch.DrawString(hudFont, title, titlePosition, Color.White);
        _spriteBatch.DrawString(hudFont, instruction, instructionPosition, Color.White);
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

        const string title = "YOU     WIN!";
        string scoreText = $"YOUR     SCORE     IS     {gameSession.Score:D5}";
        const string thanks = "THANKS     FOR     PLAYING!!!";

        DrawCenteredText(title, boxY + 18, logicalWidth, Color.White);
        DrawCenteredText(scoreText, boxY + 50, logicalWidth, Color.White);
        DrawCenteredText(thanks, boxY + 82, logicalWidth, Color.White);
    }

    private void DrawCenteredText(string text, float y, float screenWidth, Color color)
    {
        Vector2 textSize = hudFont.MeasureString(text);
        Vector2 position = new Vector2((screenWidth - textSize.X) / 2, y);

        _spriteBatch.DrawString(hudFont, text, position, color);
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
        player = loadedLevel.Player;
        levelState = loadedLevel.LevelState;
        entityManager = loadedLevel.EntityManager;

        gameplayController.StartLevel();
        return true;
    }
}
