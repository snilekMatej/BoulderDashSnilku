using BoulderDashSnilku.Input;
using BoulderDashSnilku.World;
using BoulderDashSnilku.Entities;
using BoulderDashSnilku.Simulation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BoulderDashSnilku.Core;

public class Game1 : Game
{
    private GameWorld world;
    private Player player;

    private PlayerLogic playerLogic;
    private InputManager _input;

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
        // TODO: Add your initialization logic here
        world = new GameWorld();
        player = new Player(2, 4);

        playerLogic = new PlayerLogic();
        _input = new InputManager();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update(gameTime);

        MoveDirection direction = _input.GetMoveDirection();
        playerLogic.Update(player, world, direction);

        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        int TileSize = 16;

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
                    case Tile.Boulder: color = Color.DarkSlateGray; break;
                    case Tile.Gem: color = Color.Yellow; break;
                    case Tile.Empty: color = Color.Black; break;
                }

                _spriteBatch.Draw(pixel, new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize), color);
            }
        }
        // Draw PLayer
        _spriteBatch.Draw(pixel, new Rectangle(player.x * TileSize, player.y * TileSize, TileSize, TileSize), Color.Cyan);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
