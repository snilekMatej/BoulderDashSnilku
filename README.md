# Boulder Dash

## Specification

My project is a recreation of the original Boulder Dash game.

The goal of the game is to move through a level, collect the
required number of gems and reach the exit before the time runs out.
The player has to avoid enemies and falling boulders while moving
through the map.

The project recreates the main gameplay mechanics of Boulder Dash
while using my own graphics and implementation.

The game was written in C# using the MonoGame framework.

## Main Features

The game contains:

- five playable levels based on the original Boulder Dash,
- player movement using arrow keys,
- falling and rolling boulders,
- falling and rolling gems,
- two enemy types - Firefly and Butterfly,
- enemy and player explosions,
- gem collection,
- score system,
- gem quota for opening the exit,
- time limit,
- four player lives,
- start screen,
- win screen,
- custom bitmap font,
- levels loaded from external text files.

The game uses original graphics inspired by Boulder Dash and old
ZX Spectrum games.

The complete level is displayed on screen instead of using the
scrolling camera of the original game.

## Controls

The game is controlled using the keyboard.

- `Up Arrow` - move up
- `Down Arrow` - move down
- `Left Arrow` - move left
- `Right Arrow` - move right
- `Enter` - confirm / start the game / leave the win screen

## Gameplay

The objective of every level is to collect a required number of gems.
The required number is defined separately for each level. After
enough gems are collected, the exit opens. Entering the exit
completes the current level and loads the next one.

Remaining time is converted into additional score when a level is
completed. The player starts with four lives. When the player dies
and still has lives remaining, the current level is restarted.
When all lives are lost, the player returns to the start screen.

After completing the fifth level, the final score and a thank-you
message are displayed.

## Levels

Levels are stored as text files inside:

```
Content/Levels/
```

The game contains:

```
Level00.txt
Level01.txt
Level02.txt
Level03.txt
Level04.txt
Level05.txt
```

`Level00.txt` is a development level used for testing game mechanics.
The actual game starts with `Level01.txt` and ends after `Level05.txt`.

Every normal level contains metadata followed by its map layout.
Metadata contain:

```
QUOTA=10
GEMVALUE=5
TIME=150
```

`QUOTA` defines the number of gems required to open the exit.

`GEMVALUE` defines how many points are awarded for collecting a gem.

`TIME` defines the time available for completing the level.

The level files are external files used by the game and
normally do not need to be modified.

## Installation and Running

The project requires .NET and MonoGame.

The project can be opened using the included Visual Studio solution:
`BoulderDashSnilku.sln`

The game can then be built and started from Visual Studio.
Alternatively, from the project directory it can be built using:
`dotnet build` and run using: `dotnet run` The game runs in
windowed mode.

## Project Structure

The source code is separated into several folders according to
responsibility.

```
Core/
Entities/
Input/
Library/
Rendering/
Simulation/
World/
Content/
```

`Core` contains the main game and gameplay control.

`Entities` contains the player and enemy classes.

`Input` contains keyboard input handling.

`Library` contains shared constants, directions and utility functions.

`Rendering` contains classes responsible for drawing the world, entities and HUD.

`Simulation` contains the main gameplay logic including player movement, enemies, falling objects and explosions.

`World` contains the game world, tiles and level loading.

`Content` contains graphics, fonts and level files.

## Graphics

The game uses 16 x 16 pixel tiles. The graphical assets were
created specifically for this project and are stored in the
`Content` folder. The visual style is inspired by the original
game and ZX Spectrum games, although normal RGB colors are used
instead of reproducing the exact hardware limitations.

## Documentation

link[...].

It describes the project architecture, world representation,
game loop, physics, level system, rendering, testing and possible
future improvements. The documentation is also written in English.