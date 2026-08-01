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
        worldRenderer.Draw(_spriteBatch, world, TileSize, HudHeight);
        // Draw PLayer
        playerRenderer.Draw(_spriteBatch, player, TileSize, HudHeight);
        // Draw Entities
        entityRenderer.Draw(_spriteBatch, entityManager, TileSize, HudHeight);
        // Draw HUD
        hudRenderer.Draw(_spriteBatch, gameSession, levelState);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
    private void DrawStartScreen()
    {
        const string title = "BOULDER DASH";
        const string instruction = "PRESS ENTER TO START";

        Vector2 titleSise = hudFont.MeasureString(title);
        Vector2 instructionSize = hudFont.MeasureString(instruction);

        float logicalScreenWidth = _graphics.PreferredBackBufferWidth / PixelScale;
        float logicalScreenHeight = _graphics.PreferredBackBufferHeight / PixelScale;

        Vector2 titlePosition = new Vector2((logicalScreenWidth - titleSise.X) / 2, logicalScreenHeight / 2 - 24);
        Vector2 instructionPosition = new Vector2((logicalScreenWidth - instructionSize.X) / 2, logicalScreenHeight / 2 + 8);

        hudFont.DrawText(_spriteBatch, title, titlePosition, Color.White);
        hudFont.DrawText(_spriteBatch, instruction, instructionPosition, Color.White);
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

        DrawCenteredText(title, boxY + 18, logicalWidth, Color.White);
        DrawCenteredText(scoreText, boxY + 50, logicalWidth, Color.White);
        DrawCenteredText(thanks, boxY + 82, logicalWidth, Color.White);
    }

    private void DrawCenteredText(string text, float y, float screenWidth, Color color)
    {
        Vector2 textSize = hudFont.MeasureString(text);
        Vector2 position = new Vector2((screenWidth - textSize.X) / 2, y);

        hudFont.DrawText(_spriteBatch, text, position, color);
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
