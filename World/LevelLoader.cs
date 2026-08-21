using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System;
using System.Security.Cryptography.X509Certificates;
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
            int mapStart = FindMapStart(lines);
            Dictionary<string, string> metadata = ReadMetadata(lines, mapStart);

            string[] mapLines = lines[(mapStart + 1)..];
            ValidateLevel(mapLines);
            int gemQuota = GetMetadataInt(metadata, "QUOTA");
            int gemValue = GetMetadataInt(metadata, "GEMVALUE");
            int time = GetMetadataInt(metadata, "TIME");
            GameWorld world = new GameWorld();
            EntityManager entityManager = new EntityManager();
            Player? player = null;
            int exitX = -1;
            int exitY = -1;
            for (int y = 0; y < world.Height; y++) {
                for (int x = 0; x < world.Width; x++) {
                    char symbol = mapLines[y][x];
                    Tile? tile = GetTile(symbol);
                    if (tile != null) world.Grid[x, y] = tile.Value;
                    else
                        switch (symbol) {
                            case 'P':
                                if (player != null)
                                    throw new InvalidDataException(
                                        $"The level contains multiple player spawns.");
                                player = new Player(x, y);
                                entityManager.Add(player);
                                world.Grid[x, y] = Tile.Empty;
                                break;
                            case 'H':
                                entityManager.Add(new Firefly(x, y));
                                world.Grid[x, y] = Tile.Empty;
                                break;
                            case 'X':
                                entityManager.Add(new Butterfly(x, y));
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
        /// Find "MAP" marker that separates metadata from world map.
        /// </summary>
        /// <returns>Line index containing "MAP".</returns>
        private static int FindMapStart(string[] lines) {
            int mapStart = -1;
            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].Trim().Equals("MAP", StringComparison.OrdinalIgnoreCase))
                    mapStart = i;
            }
            if (mapStart < 0)
                throw new InvalidDataException("The level doesn't contain a MAP marker.");
            return mapStart;
        }

        /// <summary>
        /// Read KEY=VALUE metadata.
        /// Blank lines are ignored.
        /// All metadata are located before MAP mark.
        /// </summary>
        /// <returns>Dictionary [Key : Value] </returns>
        /// <exception cref="InvalidDataException"></exception>
        private static Dictionary<string, string> ReadMetadata(string[] lines, int mapStart) {
            Dictionary<string, string> metadata = new Dictionary<
                string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < mapStart; i++) {
                string line = lines[i];
                if (line.Length > 0) {
                    int separator = line.IndexOf('=');
                    if (separator <= 0)
                        throw new InvalidDataException($"Invalid metadata line '{line}'.");
                    string key = line[..separator].Trim();
                    string value = line[(separator + 1)..].Trim();
                    if (metadata.ContainsKey(key))
                        throw new InvalidDataException($"Metadata '{key}' is defined multiple times.");
                    metadata.Add(key, value);
                }
            }
            return metadata;
        }

        /// <summary>
        /// Read required metadata value;
        /// </summary>
        /// <param name="metadata">Metadata collected from level file.</param>
        /// <param name="key">Name of required metadata entry.</param>
        /// <returns>Parsed integer value.</returns>
        /// <exception cref="InvalidDataException"></exception>
        private static int GetMetadataInt(Dictionary<string, string> metadata, string key) {
            int value = -1;
            if (!metadata.TryGetValue(key, out string? text))
                throw new InvalidDataException($"Missing required metadata '{key}'.");
            if (!int.TryParse(text, out value) || value < 0)
                throw new InvalidDataException($"Invalid value for '{key}': '{text}'.");
            return value;
        }

        /// <summary>
        /// Verify that the map has correct dimensions.
        /// </summary>
        /// <exception cref="InvalidDataException"></exception>
        private static void ValidateLevel(string[] lines) {
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

        private static Tile? GetTile(char symbol) {
            return symbol switch {
                '.' => Tile.Empty,
                '-' => Tile.Dirt,
                'W' => Tile.Wall,
                'B' => Tile.Border,
                'O' => Tile.Boulder,
                'G' => Tile.Gem,
                _ => null
            };
        }
    }
}
