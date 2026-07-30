using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.World
{
    public class LevelLoader
    {
        public const int DefaultGemQuota = 20;

        public static LoadedLevel Load(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            int gemQuota = ParseGemQuota(lines[0]);
            string[] mapLines = lines[1..];

            ValidateLevel(mapLines);

            int width = mapLines[0].Length;
            int height = mapLines.Length;

            GameWorld world = new GameWorld();

            Player player = null;

            int exitX = -1;
            int exitY = -1;

            EntityManager entityManager = new EntityManager();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    char symbol = mapLines[y][x];

                    switch (symbol)
                    {
                        case '.':
                            world.Grid[x, y] = Tile.Empty;
                            break;
                        case '-':
                            world.Grid[x, y] = Tile.Dirt;
                            break;
                        case 'W':
                            world.Grid[x, y] = Tile.Wall;
                            break;
                        case 'B':
                            world.Grid[x, y] = Tile.Border;
                            break;
                        case 'O':
                            world.Grid[x, y] = Tile.Boulder;
                            break;
                        case 'G':
                            world.Grid[x, y] = Tile.Gem;
                            break;
                        case 'P':
                            player = new Player(x, y);
                            entityManager.Add(player);
                            world.Grid[x, y] = Tile.Empty;
                            break;
                        case 'H':
                            Firefly firefly = new Firefly(x, y);
                            entityManager.Add(firefly);
                            world.Grid[x, y] = Tile.Empty;
                            break;
                        case 'X':
                            Butterfly butterfly = new Butterfly(x, y);
                            entityManager.Add(butterfly);
                            world.Grid[x, y] = Tile.Empty;
                            break;
                        case 'E':
                            exitX = x;
                            exitY = y;
                            world.Grid[x, y] = Tile.Border;
                            break;
                        default:
                            throw new InvalidDataException(
                                $"Unknown level symbol '{symbol}' at ({x}, {y}).");
                    }
                }
            }

            if (player == null)
            {
                throw new InvalidDataException(
                    "The level doesn't contain a player spawn.");
            }
            if (exitX < 0 || exitY < 0)
            {
                throw new InvalidDataException(
                    "The level doesn't contain an exit.");
            }

            LevelState levelState = new LevelState(exitX, exitY, gemQuota);

            return new LoadedLevel(world, player, levelState, entityManager);
        }
        private static int ParseGemQuota(string line)
        {
            const string prefix = "QUOTA=";

            if (!line.StartsWith(prefix))
            {
                throw new InvalidDataException($"The first line must use the format {prefix}<number>.");
            }
            string quotaText = line[prefix.Length..];

            if (!int.TryParse(quotaText, out int quota) || quota < 0)
            {
                throw new InvalidDataException($"Invalid gem quota '{quotaText}'.");
            }
            return quota;
        }
        private static void ValidateLevel(string[] lines)
        {
            if (lines.Length == 0)
            {
                throw new InvalidDataException("The level file is empty.");
            }
            int expectedWidth = 40;
            int expectedHeight = 22;
            if (lines.Length != expectedHeight)
            {
                throw new InvalidDataException(
                    $"The level height is invalid: y = {lines.Length} != {expectedHeight}");
            }
            for (int y = 0; y < lines.Length; y++)
            {
                if (lines[y].Length != expectedWidth)
                {
                    throw new InvalidDataException($"The level width is invalid: line({y}).width = {lines[y].Length} != {expectedWidth}");
                }
            }
        }
    }
}
