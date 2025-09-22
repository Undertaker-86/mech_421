using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platformer
{
    public class Player
    {
        private Vector2 position;
        private Vector2 velocity;
        private Texture2D texture;
        private Rectangle bounds;
        private Health health;
        
        // Player properties
        private const float Speed = 200f;
        private const float JumpPower = 400f;
        private const float Gravity = 800f;
        private const float GroundFriction = 0.8f;
        private const float AirResistance = 0.95f;
        
        // Sprite animation properties
        private const int FrameWidth = 32; // Assuming 32x32 frames in the spritesheet
        private const int FrameHeight = 32;
        private const int PlayerRow = 1; // Second row (0-indexed)
        private const float SpriteScale = 2.0f; // Scale sprites to 2x their original size
        private int currentFrame = 0;
        private float frameTimer = 0f;
        private const float FrameTime = 0.2f; // 200ms per frame
        private bool facingLeft = false;
        
        private bool isOnGround;
        
        public Vector2 Position => position;
        public Rectangle Bounds => new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height);
        public Health Health => health;
        public bool IsAlive => health.IsAlive;
        
        public Player(Vector2 startPosition, Texture2D playerTexture)
        {
            position = startPosition;
            texture = playerTexture;
            bounds = new Rectangle(0, 0, (int)(32 * SpriteScale), (int)(32 * SpriteScale)); // Scaled bounds
            velocity = Vector2.Zero;
            health = new Health(100); // 100 HP
        }
        
        public void Update(GameTime gameTime, Level level)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboardState = Keyboard.GetState();
            
            bool isMoving = false;
            
            // Horizontal movement
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                velocity.X = -Speed;
                facingLeft = true;
                isMoving = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                velocity.X = Speed;
                facingLeft = false;
                isMoving = true;
            }
            else
            {
                // Apply friction
                velocity.X *= isOnGround ? GroundFriction : AirResistance;
            }
            
            // Jumping
            if ((keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) && isOnGround)
            {
                velocity.Y = -JumpPower;
                isOnGround = false;
            }
            
            // Apply gravity
            velocity.Y += Gravity * deltaTime;
            
            // Calculate movement for this frame
            Vector2 frameVelocity = velocity * deltaTime;
            
            // Check collision with level
            Vector2 newPosition;
            Vector2 newVelocity;
            bool groundContact;
            
            if (level.CheckCollision(Bounds, frameVelocity, out newPosition, out newVelocity, out groundContact))
            {
                position = newPosition;
                velocity = newVelocity / deltaTime; // Convert back from frame velocity
                isOnGround = groundContact;
            }
            else
            {
                // No collision, update position normally
                position += frameVelocity;
                isOnGround = false;
            }
            
            // Keep player on screen (left and right bounds)
            if (position.X < 0)
                position.X = 0;
            else if (position.X > 800 - bounds.Width) // Use fixed screen width
                position.X = 800 - bounds.Width;
            
            // Update animation
            if (isMoving && isOnGround)
            {
                frameTimer += deltaTime;
                if (frameTimer >= FrameTime)
                {
                    frameTimer = 0f;
                    currentFrame = (currentFrame + 1) % 4; // Assuming 4 walking frames
                }
            }
            else
            {
                currentFrame = 0; // Idle frame
                frameTimer = 0f;
            }
        }
        
        public void TakeDamage(int damage)
        {
            health.TakeDamage(damage);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsAlive)
            {
                Rectangle sourceRectangle = new Rectangle(
                    currentFrame * FrameWidth,
                    PlayerRow * FrameHeight,
                    FrameWidth,
                    FrameHeight
                );
                
                SpriteEffects effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, position, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteScale, effects, 0f);
            }
        }
    }
}
