# Boulder Dash Documentation

## 1. Introduction

This document describes the implementation of my recreation of the
original Boulder Dash. The project was developed in C# using the
MonoGame framework.

The main goal of the project was to recreate the gameplay of the
original game while using my own implementation and graphics.
Even though the mechanics are inspired by the original game,
some technical parts were simplified to keep the project
manageable and easier to maintain.

Instead of copying the original graphics, I created my own sprites
inspired by the original. Some of them resemble the original game
while others were designed in my own style. The visual part also
tries to imitate the limited-color look of ZX Spectrum games while
still using modern RGB colors.

The project focuses mainly on recreating the gameplay mechanics,
including falling boulders, collectible gems, enemies, explosions
and progression through multiple levels.

---

## 2. Project Goal

The main objective of the project was to recreate the gameplay
mechanics of the original game as close as possible while
implementing the entire game from scratch.

The main focus was placed on recreating:

- player movement,
- falling and rolling boulders and gems,
- enemy behaviour,
- explosions,
- gem collection,
- level progression.

The graphics are not intended to be an exact copy. Instead,
they are an original interpretation inspired by Boulder Dash
and other games.

Only the first five original Boulder Dash levels were implemented
because they provide enough variety while keeping the project
at a reasonable size.

---

## 3. Technologies Used

The game was made in **C#**.

The game uses **MonoGame**, which provides graphics rendering,
input handling and the main game loop.

The project was developed in **Visual Studio**.

All graphics used in the project were created only
for this game.
No external graphical assets were used.
The project does not contain sound effects or music because
the main goal was to complete the gameplay mechanics first.

---

## 4. Project Architecture

To make the project easier to understand, the source code is
divided into several folders. Every folder has a specific
responsibility. This makes it easier to locate a particular
section of the implementation without searching through a
single large file.

The central file of the project is `Game1.cs`. It is responsible
for creating the game window, initializing the game, running
the game loop and connecting all other parts of the project together.
Instead of containing all gameplay logic, it acts mainly
as a coordinator that calls the individual systems.

The project is divided into the following modules.

### Core

The **Core** folder contains the files responsible for
controlling the game itself.

Classes:

- `Game1.cs` - main game loop.
- `GameplayController.cs` - controls the gameplay flow.
- `GameplayState.cs` - stores the current gameplay state.
- `GameSession.cs` - stores information about the current game session.
- `GameState.cs` - controls which screen is currently active.

The game uses a simple state system. Depending on the current state,
the program displays either the start screen, gameplay or the
final screen.

### Entities

The **Entities** folder contains all game entities.

Implemented entities include:

- Player
- Firefly
- Butterfly

All entities inherit from common base class `Entity`, allowing
the game to update different entity types in a similar way.

The `EntityManager` keeps track of all active entities currently
present in the level.

### Input

The **Input** folder contains the keyboard input system.

Only arrow keys are used for player movement. The Enter key is used
as a confirmation button for entering the game from the start
screen and returning from the final screen. The Escape key is
used to exit the program at any point.

### Rendering

Rendering is divided into multiple renderer classes
instead of drawing everything inside the main game class.

The rendering system consists of:

- `WorldRenderer`
- `PlayerRenderer`
- `EntityRenderer`
- `HudRenderer`
- `BitmapFont`

Separating rendering into several classes makes the drawing
code easier to read and allows every renderer to focus on
a single responsibility.

### Simulation

Most gameplay logic is implemented inside the **Simulation** folder.

Instead of having one large update function, the simulation
is divided into several independent systems.

These include:

- player movement,
- enemy movement,
- boulder behaviour,
- gem behaviour,
- falling object logic,
- explosion logic,
- world simulation.

This separation made the code easier to maintain as the project
became larger.

### World

The **World** folder contains everything related to the game map.

Important classes include:

- `GameWorld` stores the array of the world.
- `LevelLoader` loads the level from the level file.
- `LoadedLevel` stores metadata of the level.
- `LevelState` stores the state of the level gameplay.
- `Tile` stores all tile variants.

The world stores the terrain, loads level files and keeps track of
information needed during gameplay.

### Library

The **Library** folder contains helper classes that are shared
across the whole project.

These include:

- constants,
- movement directions,
- utility functions.

Keeping these values in one place avoids using magic numbers
throughout the project and makes future modifications easier.

### Overall design

One of the main design decisions was to separate
terrain from entities.

The terrain is stored independently from moving objects
such as the player or enemies. This makes the code easier
to understand because terrain tiles and entities represent
different concepts.

Another important design decision was to keep each subsystem
responsible only for its own task. Rendering does not modify
gameplay logic, the simulation does not perform rendering
and input handling is separated from both. This reduces
dependencies between different parts of the project
and makes the implementation easier to extend.

---

## 5. World Representation

The game world is represented using several independent data structures.
Instead of storing all information in one place, the terrain,
entities and simulation state are separated. This makes the
implementation easier to understand and allows every part of
the game to work with only the data it needs.

### Terrain

The terrain is stored as a 2D array of `Tile` values.

Each value represents one type of terrain:

- Empty
- Dirt
- Wall
- Border
- Boulder
- Gem
- Exit

The tile type determines how the game should interact with that
position. Most gameplay decisions are based on the tile type.

### Simulation State

Besides the terrai, the simulation stores additional 2D arrays
to track falling objects. One remembers which objects were falling
in previous state and another temporary array with already processed
positions in current update.

Remembering which tiles were already processed is necessary
because the world is updated tile by tile. Without this,
a boulder could be updated multiple times during a single frame.

The falling state is also important because only falling
objects are able to eliminate entities.

### Entities

Moving objects are not stored directly inside the terrain.

Instead, the game keeps a separate collection of entities.
This collection contains the player and all enemies in the level.

Each entity stores its own information internally, such
as its position and current state. This removes the need
for multiple parallel arrays and keeps all information
related to an entity together.

Separating entities from terrain also simplifies collision
detection because terrain and moving objects are treated
as two different systems.

### Special Objects

Most tiles only require the information stored inside
the terrain array. Some objects require additional information.

The exit remembers its position so it can later change from
a closed exit into an open exit after the player collects enough gems.

Boulders and gems also store additional simulation
information, such as whether they are currently falling.

### Level Loading

Levels are stored as plain text files.

Each file consists of two sections:

1. Level metadata.
2. Map layout.

The metadata currently contains:

- `QUOTA` – number of gems required to open the exit.
- `GEMVALUE` – score awarded for collecting one gem.
- `TIME` – time limit for that level.

The metadata uses a simple key-value format. The map 
is then read line by line. Each character represents
one tile. During loading, the corresponding terrain
tile is created. If the loaded tile represents the player
or an enemy, an entity is created instead, while the
terrain at that position becomes an empty tile.

The exit behaves differently. Its position is remembered
separately so that it can later change from a closed exit
into an open exit when the required number of gems
has been collected.

The level number itself is not stored inside the level file.
Instead, it is determined from the file name. Since every level
has identical dimensions, the width and height are fixed and
so they do not need to be stored inside the file.

---

## 6. Game Loop

The game is controlled by two main methods: `Update()` and `Draw()`.

The `Update()` method is responsible for all game logic,
while the `Draw()` method only renders the current game state.
Separating these two responsibilities makes the rendering
independent from the gameplay logic.

### Update

During every frame, the game performs the following operations:

1. Read keyboard input.
2. Update the player and resolve player collisions.
3. Update all falling objects.
4. Update all remaining entities.
5. Resolve special events if necessary.

The player is always updated before other entities.
Collisions are resolved immediately when an object attempts
to move. Instead of performing a separate collision detection
after all objects have moved, every object checks whether its
movement is valid before changing its position.

This approach simplifies the implementation because all
object are responsible for their own movement and
collision handling.

### Special Events

Some situations require the normal update process to stop temporarily.
When the player dies or player loses all lives the game has to resolve
the case immediately.

### Draw

After the update phase finishes, the `Draw()` method renders
the current game state. Rendering is divided into several
renderers for each type.

The world is drawn first, followed by entities and then
the user interface.

The user interface displays:

- remaining lives,
- current score,
- gem quota,
- value of collected gems,
- remaining time.

Since rendering is completely separated from the gameplay logic,
changing the appearance of the game does not require modifying
the simulation itself.

### Game States

The game is controlled using a simple state system.

Three main states are implemented:

- Start Screen
- Gameplay
- End Screen

Depending on the current state, the game executes different
update and rendering logic.

For example, during the start screen the game waits for
the player to press the Enter key, while during gameplay
it updates the world simulation. After the final level
is completed, the game switches to the win screen,
where the final score and a thank-you message are displayed.

---

## 7. Physics and Algorithms

The most difficult part of the project was implementing the
game logic. While drawing the game and loading levels
were relatively straightforward, recreating the
behaviour of boulders, gems and explosions required
much more attention.

The implementation was gradually improved during development.
At first, most of the physics was contained inside a single
update function. As the project grew, the simulation was
divided into several smaller systems, making the code easier
to read and maintain.

### Boulder Physics

Boulders are processed during every simulation update.

The world is scanned tile by tile. If a boulder is found,
the game checks whether it can move. First, the tile directly
below the boulder is checked. The game uses the concept of
a **truly empty tile**. Such a tile must be `Tile.Empty` and
has no entity at its position. If the tile below is truly empty,
the boulder is marked as **falling** which allows it to fall next update.

If the tile below is occupied by an entity, the boulder
cannot fall, unless the boulder was **falling**. Then the entity
below the boulder is then killed.

Otherwise it can't fall. However, under certain conditions
it may roll to the side. Rolling is only attempted when the
tile below the boulder is another boulder, a gem, a wall or
the map border. The game then checks whether the neighbouring
tile and the tile below that tile are both truly empty. If
both positions are free, the boulder rolls to that side
and continues falling during the following updates.

If neither vertical movement nor rolling is possible,
the boulder remains on that tile.

### Gem Physics

Gems use exactly the same movement rules as boulders.

The only difference is when the player moves onto a tile with gem,
the gem is then collected and is removed from the tile.

### Processed Tiles

One problem discovered during development was caused by the
update order. At first, the simulation processed the world
from one side to the other without remembering which
tiles had already been updated. Because of that, boulders
on one side of the map would fall faster than those on the
other side because some objects were processed multiple times
during a single update.

During each simulation update, a separate array remembers already
peocessed positions. This guarantees that every falling
object is updated only once during each update.

### Enemy Behaviour

Two enemy types are implemented:

- Firefly
- Butterfly

Their behaviour is based on the original Boulder Dash. 
Enemies move along obstacles by following edges of solid tiles.
One small difference from the original game is that enemies
immediately begin following walls instead of first moving
up. When an enemy collides with the player or player moves into
an enemy, the player immediately dies.

### Explosions

Explosions occur whenever any entity dies. The explosion
affects 3x3 area centered on entity. Entities inside that
area are also killed, which makes them explode as well.
For enemies destructable tiles are turned into gems and
Player leaves the tiles empty. Border tiles and the exit
cannot be destroyed.

### Design Decisions

One important lesson learned during development was that
splitting the simulation into several independent systems
makes the implementation much easier to understand.

Instead of having one large function responsible for every
gameplay mechanic, each system now focuses on a single task
such as player movement, explosions or falling objects.

This approach also made adding new mechanics simpler
because each feature could be implemented in its own class
without making the rest of the simulation unnecessarily complicated.

---

## 8. Gameplay and Level Progression

The game contains five playable levels based on the first
five levels of the original Boulder Dash.

The main objective of each level is to collect a required number
of gems and reach the exit before the time limit expires.

### Gem Collection

Every level defines a gem quota using the `QUOTA` value stored
in its level file.

Collecting a gem increases the player's score according to
the `GEMVALUE` setting of the current level.

Once the required number of gems has been collected, the exit opens.
The exit is initially represented by a border tile. Its position
is remembered when the level is loaded. After the quota is reached,
this tile is replaced by an open exit.

### Completing a Level

The level is completed when the player enters the open exit.
Any remaining time is then converted into additional score.
Every remaining second adds one point. After this, the next level
is loaded.

### Lives and Death

The player starts the game with four lives. The player can die by:

- touching an enemy,
- being crushed by a falling object,
- running out of time.

If the player still has lives remaining after dying, the
current level is restarted. When all lives are lost, the game
returns to the start screen. After completing the fifth and
final level, the game displays the win screen. The final score
and a short thank-you message are displayed there. Pressing
Enter returns the player to the start screen.

---

## 9. Graphics and Rendering

The graphics were created specifically for this project. Every
terrain tile is represented by a separate 16 x 16 pixel image
stored in the `Content` `/{Tiles; Player; Enemies}` directory.
Separate images are also used for the player and both enemy types.

The game contains graphics for:

- dirt,
- empty space,
- walls,
- borders,
- boulders,
- gems,
- exit,
- player,
- Firefly,
- Butterfly.

The graphics are inspired by the original Boulder Dash, but
they are not direct copies. I wanted the game to resemble the
appearance of old ZX Spectrum games. Normal RGB colors are used,
allowing more freedom when creating the graphics while keeping
a similar visual style.

### Level Rendering

Unlike the original Boulder Dash, the entire level is visible
on screen at the same time. The original game uses a scrolling
view and only displays a part of the level around the player.

### HUD

During gameplay, the HUD displays all important information
about the current game. It shows:

`{LIVES}{ALT:03-HEART}   {GEMQUOTA} / {GEMVALUE}   SCORE:{SCORE}   TIME:{TIME}`

- remaining lives,
- gem quota,
- gem value,
- current score,
- remaining time.

The game also contains a custom bitmap font used for displaying text
from old BIOS 8x8 font.

### Window

The game currently runs only in windowed mode. Fullscreen support
is not implemented.

---

## 10. Controls

The controls were intentionally kept simple.

### Movement

The player is controlled using the arrow keys:

- `Up Arrow` - move up
- `Down Arrow` - move down
- `Left Arrow` - move left
- `Right Arrow` - move right

The player can hold a movement key to continue moving in the
selected direction.

### Menu Controls

The Enter key is used as the confirmation key. On the start
screen, pressing Enter starts the game and loads the first level.
After completing the final level, pressing Enter on the win
screen returns the player to the start screen.

The Escape key is used to instantly close the program. There is
no save feature so all progress is lost.

---

## 11. Development and Differences

### Testing and Debugging

Testing was performed mainly by manually playing the game and
checking individual gameplay situations. In addition to the
five normal levels, the project contains a special `Level00.txt`.
Level 00 is not part of the normal game. It was created as a
testing environment where individual mechanics can be placed
into specific situations without modifying one of the real levels.
The normal game always starts with Level 01.

The test level was useful for testing mechanics such as:

- falling boulders,
- rolling objects,
- collisions,
- enemies,
- explosions,
- player movement.

Visual Studio debugging tools were also used during development.
Breakpoints were useful when checking the current state of the
world and following the execution of individual functions.
Compiler and runtime error messages were used to locate problems
in the code.

---

## 12. Differences From the Original Game

The objective was to reproduce the main gameplay of Boulder Dash,
but the recreation is not an exact copy. Several differences were
intentionally left in the final implementation.

### Graphics

All graphics were created for this project instead of copying
the original assets. They are inspired by Boulder Dash and
the ZX Spectrum style but use normal RGB colors.

### Camera

The original game uses a scrolling camera and displays only part
of the level. This recreation displays the complete level at once.
This makes navigation easier because the player can always see
the complete map.

### Enemy Movement

Enemy movement is based on the original behaviour, but there
is a small difference when an enemy begins moving. In the
original game, the enemy first moves upward before starting
to follow walls. In this implementation, enemies begin
following walls immediately.

### Animations and Sound

The game currently contains no sprite animations, sound effects
or music. These features are not required for the main gameplay
and were therefore left out of the implementation.

### Additional Interface Features

There is currently:

- no pause menu,
- no separate Game Over screen,
- no fullscreen mode,
- no high-score system,
- no level selection.

These features do not affect the main objective of recreating
the Boulder Dash gameplay.

---

## 13. Conclusion

The goal of this project was to recreate the main gameplay of the
original Boulder Dash using C#.
The project implements the main mechanics required to play the game,
including player movement, falling and rolling objects, enemies,
explosions, gem collection, scoring, lives, level progression and
some playable levels.

One of the most important parts of the project was implementing
the behaviour of falling objects. The simulation had to ensure
that every object was processed only once during an update while
also remembering whether objects were falling.

Another important part was organizing the project into separate
systems. Keeping entities, terrain, rendering, input and
simulation separate made the code easier to understand and
modify as new mechanics were added.

During development I learned that it is useful to first create
a simple working version of the game and then gradually improve
individual systems. In this project, creating the world and basic
gameplay first made it possible to test the important
mechanics before spending time on menus and other secondary features.

The finished game is not an exact recreation of the original
game, but it reproduces its main gameplay while using my own
graphics and implementation.