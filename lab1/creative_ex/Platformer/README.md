# 2D Platformer Game

A complete 2D platformer game built with MonoGame framework featuring platforms, sprites, enemies, health system, combat mechanics, and level progression.

## Features

- **Multi-Level Platform System**: Navigate through brown platforms and green ground
- **Player Character**: 2x scaled animated sprite from the second row of the character spritesheet
- **Enemy System**: 2x scaled goblin enemies that patrol platforms intelligently
- **Health System**: Visual health bars for both player and enemies
- **Combat System**: 2x scaled saber attack with animated sword slash sprites
- **Physics**: Advanced gravity, jumping, and platform collision detection
- **Game States**: Playing, Death, and Level Complete states with retry system
- **Level Design**: Multiple platforms at different heights creating a challenging level

## Controls

- **Movement**: 
  - Move Left: `A` or `Left Arrow`
  - Move Right: `D` or `Right Arrow`
- **Jumping**: 
  - Jump: `W`, `Up Arrow`, or `Space`
- **Combat**: 
  - Attack: `X` key (saber slash)
- **Game States**:
  - Retry/Restart: `R` key (when dead or level complete)
- **Exit**: `Escape` key

## Game Mechanics

- **Player Health**: 100 HP (green health bar at top)
- **Enemy Health**: 50 HP each (smaller health bars below player's)
- **Saber Damage**: 25 damage per hit (increased range for larger sprites)
- **Enemy Contact Damage**: 10 damage to player when touching enemies
- **Enemy AI**: Intelligent platform patrol, changes direction at edges and walls
- **Level Progression**: Complete levels by defeating all enemies
- **Respawn System**: Press R to retry when you die or complete the level

## Level Design

The game features a carefully designed level with multiple platforms:
- Ground level (green)
- 7 brown platforms at various heights
- Strategic placement encourages jumping and combat
- Enemies spawn on different platforms for varied gameplay

## Sprite Assets

The game uses three main sprite files:
- `characters.png` - Player character spritesheet (uses second row)
- `goblin.png` - Enemy sprite
- `pixel_art_sword_slash_sprites.png` - Animated sword attack effects

## Development

This project can be developed entirely in Visual Studio Code. To run:

```bash
dotnet run --project Platformer.csproj
```

To build:

```bash
dotnet build Platformer.csproj
```

## Future Enhancements

Potential features to add:
- Sound effects and background music
- Multiple levels with platforms
- Power-ups and collectibles
- More enemy types
- Player respawning system
- Score system
- Better sprite animations
