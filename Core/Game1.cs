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
        currentLevel = 0; // set to 0!
        gameSession = new GameSession();

        playerLogic = new PlayerLogic();
        explosionLogic = new ExplosionLogic();

        worldSimulation = new WorldSimulation();
        _input = new InputManager();

        gameplayController = new GameplayController();
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
                LoadLevel(currentLevel);
            }
            else
            {
                worldSimulation.Update(world, entityManager, gameTime);
            }
        }

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: Matrix.CreateScale(PixelScale));

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


    private void LoadLevel(int levelNumber)
    {
        string fileName = $"Level{levelNumber:D2}.txt";
        string levelPath = Path.Combine(AppContext.BaseDirectory, "Content", "Levels", fileName);

        LoadedLevel loadedLevel = LevelLoader.Load(levelPath);

        world = loadedLevel.World;
        player = loadedLevel.Player;
        levelState = loadedLevel.LevelState;
        entityManager = loadedLevel.EntityManager;

        gameplayController.StartLevel();
    }
}
