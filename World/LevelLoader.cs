using System.IO;
using BoulderDashSnilku.Entities;

namespace BoulderDashSnilku.World {
    /// <summary>
    /// Reads level file and converts its metadata and character map into playable level information.
    /// Validates level dimensions, player spawn, exit and metadata before returning result.
    /// </summary>
    public class LevelLoader {

        /// <summary>
        /// Load and validate complete level from supplied text file.
        /// First 3 lines contain level settings and the rest is the level layout.
        /// </summary>
        /// <param name="filePath">Path of the level file to load.</param>
        /// <returns>{World, player, levelState, entityManager}</returns>
        /// <exception cref="InvalidDataException"></exception>
        public static LoadedLevel Load(string filePath) {
            string[] lines = File.ReadAllLines(filePath);
            int expectedLines = 3 + GameWorld.DefaultHeight;
            if (lines.Length != expectedLines)
                throw new InvalidDataException(
                    "The level must contain QUOTA, GEMVALUE, TIME, and map data.");
            int gemQuota = ParseGemQuota(lines[0]);
            int gemValue = ParseGemValue(lines[1]);
            int time = ParseTime(lines[2]);
            string[] mapLines = lines[3..];
            ValidateLevel(mapLines);
            GameWorld world = new GameWorld();
            EntityManager entityManager = new EntityManager();
            Player? player = null;
            int exitX = -1;
            int exitY = -1;
            for (int y = 0; y < world.Height; y++) {
                for (int x = 0; x < world.Width; x++) {
                    char symbol = mapLines[y][x];
                    switch (symbol) {
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
                            if (player != null)
                                throw new InvalidDataException(
                                    $"The level contains multiple player spawns.");
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
                            if (exitX >= 0)
                                throw new InvalidDataException(
                                    $"The level contains multiple exits.");
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
                throw new InvalidDataException(
                    "The level doesn't contain a player spawn.");
            if (exitX < 0 || exitY < 0)
                throw new InvalidDataException(
                    "The level doesn't contain an exit.");
            LevelState levelState = new LevelState(
                exitX, exitY, gemQuota, gemValue, time);
            return new LoadedLevel(world, player, levelState, entityManager);
        }

        /// <summary>
        /// Read required gem requirement from QUOTA metadata line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns>Gem quota</returns>
        /// <exception cref="InvalidDataException"></exception>
        private static int ParseGemQuota(string line) {
            const string prefix = "QUOTA=";
            if (!line.StartsWith(prefix))
                throw new InvalidDataException(
                    $"The first line must use the format {prefix}<number>.");
            string quotaText = line[prefix.Length..];
            if (!int.TryParse(quotaText, out int quota) || quota < 0)
                throw new InvalidDataException($"Invalid gem quota '{quotaText}'.");
            return quota;
        }

        /// <summary>
        /// Read required gem value from GEMVALUE metadata line.
        /// </summary>
        /// <returns>Gem value</returns>
        /// <exception cref="InvalidDataException"></exception>
        private static int ParseGemValue(string line) {
            const string prefix = "GEMVALUE=";
            if (!line.StartsWith(prefix))
                throw new InvalidDataException(
                    $"The second line must use the format {prefix}<number>.");
            string gemValueText = line[prefix.Length..];
            if (!int.TryParse(gemValueText, out int gemValue) || gemValue < 0)
                throw new InvalidDataException($"Invalid gem value '{gemValueText}'");
            return gemValue;
        }

        /// <summary>
        /// Read required remaining time from TIME metadata line.
        /// </summary>
        /// <returns>Remaining time</returns>
        /// <exception cref="InvalidDataException"></exception>
        private static int ParseTime(string line) {
            const string prefix = "TIME=";
            if (!line.StartsWith(prefix))
                throw new InvalidDataException(
                    $"The third line must use the format {prefix}<number>.");
            string timeText = line[prefix.Length..];
            if (!int.TryParse(timeText, out int time) || time < 0)
                throw new InvalidDataException($"Invalid time remaining '{timeText}'");
            return time;
        }

        /// <summary>
        /// Verify that the map has correct dimensions.
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        private static void ValidateLevel(string[] lines) {
            if (lines.Length == 0)
                throw new InvalidDataException("The level file is empty.");
            if (lines.Length != GameWorld.DefaultHeight)
                throw new InvalidDataException(
                    $"The level height is invalid: y = {lines.Length} != {GameWorld.DefaultHeight}");
            for (int y = 0; y < lines.Length; y++) {
                if (lines[y].Length != GameWorld.DefaultWidth)
                    throw new InvalidDataException(
                        $"The level width is invalid:" + 
                        $"line({y}).width = {lines[y].Length} != {GameWorld.DefaultWidth}");
            }
        }
    }
}
