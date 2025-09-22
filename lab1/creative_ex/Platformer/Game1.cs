using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Platformer;

public enum GameState
{
    Playing,
    PlayerDied,
    LevelComplete
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Player _player;
    private Saber _saber;
    private List<Enemy> _enemies;
    private Level _level;
    private GameState _gameState;
    
    // Textures
    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;
    private Texture2D _saberTexture;
    private Texture2D _pixelTexture;
    private SpriteFont _font;
    
    // Sound Effects
    private SoundEffect _moveSound;
    private SoundEffect _jumpSound;
    private SoundEffect _attackSound;
    private SoundEffect _hitSound;
    private SoundEffect _deathSound;
    
    // UI
    private HealthBar _playerHealthBar;
    private List<HealthBar> _enemyHealthBars;
    
    // Starting positions
    private Vector2 _playerStartPosition;
    private List<Vector2> _enemyStartPositions;
    
    // Sound state tracking
    private bool _wasPlayerMoving = false;
    private bool _wasPlayerOnGround = true;
    private bool _wasAttacking = false;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Set window size
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 600;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load textures
        _playerTexture = Content.Load<Texture2D>("characters");
        _enemyTexture = Content.Load<Texture2D>("goblin");
        _saberTexture = Content.Load<Texture2D>("pixel_art_sword_slash_sprites");
        
        // Create pixel texture for UI elements
        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
        
        // Create sound effects
        CreateSoundEffects();
        
        // Initialize level
        _level = new Level(GraphicsDevice, _pixelTexture);
        
        // Set starting positions
        _playerStartPosition = new Vector2(100, 200);
        _enemyStartPositions = new List<Vector2>
        {
            new Vector2(250, 200),
            new Vector2(450, 200)
        };
        
        // Initialize game
        InitializeLevel();
    }
    
    private void InitializeLevel()
    {
        _gameState = GameState.Playing;
        
        // Initialize player
        _player = new Player(_playerStartPosition, _playerTexture);
        
        // Initialize saber
        _saber = new Saber(_saberTexture, _player);
        
        // Initialize enemies
        _enemies = new List<Enemy>();
        for (int i = 0; i < _enemyStartPositions.Count; i++)
        {
            _enemies.Add(new Enemy(_enemyStartPositions[i], _enemyTexture));
        }
        
        // Initialize health bars
        _playerHealthBar = new HealthBar(_pixelTexture, new Vector2(10, 10), new Vector2(200, 20));
        _enemyHealthBars = new List<HealthBar>();
        
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemyHealthBars.Add(new HealthBar(_pixelTexture, new Vector2(10, 40 + i * 30), new Vector2(100, 15)));
        }
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        switch (_gameState)
        {
            case GameState.Playing:
                UpdatePlaying(gameTime, keyboardState);
                break;
            case GameState.PlayerDied:
                UpdatePlayerDied(keyboardState);
                break;
            case GameState.LevelComplete:
                UpdateLevelComplete(keyboardState);
                break;
        }

        base.Update(gameTime);
    }
    
    private void UpdatePlaying(GameTime gameTime, KeyboardState keyboardState)
    {
        if (_player.IsAlive)
        {
            // Track player states before update
            var previousPosition = _player.Position;
            
            _player.Update(gameTime, _level);
            _saber.Update(gameTime);
            
            // Check for sound events after update
            CheckPlayerSounds(keyboardState, previousPosition);
        }
        else
        {
            _gameState = GameState.PlayerDied;
            return;
        }
        
        // Update enemies
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            var enemy = _enemies[i];
            if (enemy.IsAlive)
            {
                enemy.Update(gameTime, _level);
                
                // Check saber collision with enemy
                if (_saber.CheckCollision(enemy.Bounds))
                {
                    enemy.TakeDamage(_saber.GetDamage());
                    _hitSound?.Play(); // Play hit sound when enemy is hit
                    
                    // Play death sound if enemy dies
                    if (!enemy.IsAlive)
                    {
                        _deathSound?.Play();
                    }
                }
                
                // Check player collision with enemy (player takes damage)
                if (_player.Bounds.Intersects(enemy.Bounds))
                {
                    _player.TakeDamage(10); // Enemy does 10 damage on contact
                    _hitSound?.Play(); // Play hit sound when player is hit
                    
                    // Play death sound if player dies
                    if (!_player.IsAlive)
                    {
                        _deathSound?.Play();
                    }
                }
            }
            else
            {
                // Remove dead enemies
                _enemies.RemoveAt(i);
                _enemyHealthBars.RemoveAt(i);
            }
        }
        
        // Check for level completion (all enemies defeated)
        if (_enemies.Count == 0)
        {
            _gameState = GameState.LevelComplete;
        }
    }
    
    private void UpdatePlayerDied(KeyboardState keyboardState)
    {
        // Press R to retry
        if (keyboardState.IsKeyDown(Keys.R))
        {
            InitializeLevel();
        }
    }
    
    private void UpdateLevelComplete(KeyboardState keyboardState)
    {
        // Press R to restart
        if (keyboardState.IsKeyDown(Keys.R))
        {
            InitializeLevel();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.SkyBlue);

        _spriteBatch.Begin();
        
        // Draw level (platforms and ground)
        _level.Draw(_spriteBatch);
        
        // Draw player
        _player.Draw(_spriteBatch);
        
        // Draw saber attack
        _saber.Draw(_spriteBatch);
        
        // Draw enemies
        foreach (var enemy in _enemies)
        {
            enemy.Draw(_spriteBatch);
        }
        
        // Draw UI - Health bars
        _playerHealthBar.Draw(_spriteBatch, _player.Health);
        
        for (int i = 0; i < _enemies.Count && i < _enemyHealthBars.Count; i++)
        {
            _enemyHealthBars[i].Draw(_spriteBatch, _enemies[i].Health);
        }
        
        // Draw game state messages
        DrawGameStateMessages();
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void DrawGameStateMessages()
    {
        Vector2 centerScreen = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        
        switch (_gameState)
        {
            case GameState.PlayerDied:
                DrawTextCentered("YOU DIED!", centerScreen, Color.Red);
                DrawTextCentered("Press R to Retry", centerScreen + new Vector2(0, 40), Color.White);
                break;
            case GameState.LevelComplete:
                DrawTextCentered("LEVEL COMPLETE!", centerScreen, Color.Gold);
                DrawTextCentered("Press R to Restart", centerScreen + new Vector2(0, 40), Color.White);
                break;
        }
    }
    
    private void DrawTextCentered(string text, Vector2 position, Color color)
    {
        // Create a simple text background
        Vector2 textSize = new Vector2(text.Length * 12, 20); // Approximate text size
        Rectangle background = new Rectangle((int)(position.X - textSize.X / 2 - 10), 
                                           (int)(position.Y - textSize.Y / 2 - 5), 
                                           (int)(textSize.X + 20), 
                                           (int)(textSize.Y + 10));
        _spriteBatch.Draw(_pixelTexture, background, Color.Black * 0.7f);
        
        // Since we don't have a font loaded, we'll use a simple text representation
        // For now, let's use rectangles to represent text
        for (int i = 0; i < text.Length; i++)
        {
            Rectangle charRect = new Rectangle((int)(position.X - textSize.X / 2 + i * 12), 
                                              (int)(position.Y - 8), 10, 16);
            _spriteBatch.Draw(_pixelTexture, charRect, color);
        }
    }
    
    private void CreateSoundEffects()
    {
        // Create programmatic sound effects
        _moveSound = CreateTone(200, 0.1f, 0.3f); // Low footstep sound
        _jumpSound = CreateTone(400, 0.2f, 0.4f); // Jump sound
        _attackSound = CreateTone(600, 0.15f, 0.5f); // Attack swoosh
        _hitSound = CreateTone(300, 0.1f, 0.6f); // Hit impact
        _deathSound = CreateTone(150, 0.5f, 0.7f); // Death sound
    }
    
    private SoundEffect CreateTone(float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int samples = (int)(sampleRate * duration);
        
        byte[] audioData = new byte[samples * 2]; // 2 bytes per 16-bit sample
        
        for (int i = 0; i < samples; i++)
        {
            double time = (double)i / sampleRate;
            double wave = Math.Sin(2 * Math.PI * frequency * time);
            
            // Add envelope to make it sound more natural
            double envelope = Math.Exp(-time * 3); // Fade out
            
            short sample = (short)(wave * envelope * volume * short.MaxValue);
            
            // Convert to bytes (little-endian)
            audioData[i * 2] = (byte)(sample & 0xFF);
            audioData[i * 2 + 1] = (byte)(sample >> 8);
        }
        
        return new SoundEffect(audioData, sampleRate, AudioChannels.Mono);
    }
    
    private void CheckPlayerSounds(KeyboardState keyboardState, Vector2 previousPosition)
    {
        // Check for movement sound (player position changed horizontally and is on ground)
        bool isMoving = (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) ||
                        keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)) &&
                        Math.Abs(_player.Position.X - previousPosition.X) > 0.1f;
        
        if (isMoving && !_wasPlayerMoving)
        {
            _moveSound?.Play(0.5f, 0f, 0f); // Play at 50% volume
        }
        _wasPlayerMoving = isMoving;
        
        // Check for jump sound (space/up/W pressed and player was on ground)
        bool jumpPressed = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W);
        if (jumpPressed && _wasPlayerOnGround && _player.Position.Y < previousPosition.Y)
        {
            _jumpSound?.Play();
        }
        
        // Update ground state for next frame (approximate check)
        _wasPlayerOnGround = _player.Position.Y >= previousPosition.Y - 1f; // Allow small tolerance
        
        // Check for attack sound
        bool isAttacking = _saber.IsAttacking;
        if (isAttacking && !_wasAttacking)
        {
            _attackSound?.Play();
        }
        _wasAttacking = isAttacking;
    }
}
