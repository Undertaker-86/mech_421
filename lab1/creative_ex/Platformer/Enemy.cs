using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer
{
    public class Enemy
    {
        private Vector2 position;
        private Vector2 velocity;
        private Texture2D texture;
        private Rectangle bounds;
        private Health health;
        private bool facingLeft;
        
        // Enemy properties
        private const float Speed = 50f;
        private const float Gravity = 800f;
        private const float SpriteScale = 1.5f; // Scale sprites to 1.5x their original size (smaller than before)
        
        // Sprite animation properties
        private const int FrameWidth = 64; // Goblin sprite frames are 64x64 (704/11 columns)
        private const int FrameHeight = 64; // Goblin sprite frames are 64x64 (320/5 rows)
        private int currentFrame = 0;
        private float frameTimer = 0f;
        private const float FrameTime = 0.3f; // 300ms per frame (slower than player)
        private const int TotalFrames = 11; // Total frames per row in the sprite sheet
        
        private bool isOnGround;
        
        public Vector2 Position => position;
        public Rectangle Bounds => new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height);
        public Health Health => health;
        public bool IsAlive => health.IsAlive;
        
        public Enemy(Vector2 startPosition, Texture2D enemyTexture)
        {
            position = startPosition;
            texture = enemyTexture;
            bounds = new Rectangle(0, 0, (int)(FrameWidth * SpriteScale), (int)(FrameHeight * SpriteScale)); // Scaled bounds
            velocity = Vector2.Zero;
            health = new Health(50); // 50 HP
            facingLeft = true;
        }
        
        public void Update(GameTime gameTime, Level level)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Simple AI - move left and right
            if (facingLeft)
            {
                velocity.X = -Speed;
            }
            else
            {
                velocity.X = Speed;
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
            
            // Change direction at screen edges or when hitting walls
            if (position.X <= 0 || velocity.X == 0)
            {
                if (position.X <= 0) position.X = 0;
                facingLeft = false;
            }
            else if (position.X >= 800 - bounds.Width) // Use fixed screen width
            {
                position.X = 800 - bounds.Width;
                facingLeft = true;
            }
            
            // Update animation
            if (isOnGround && velocity.X != 0)
            {
                frameTimer += deltaTime;
                if (frameTimer >= FrameTime)
                {
                    frameTimer = 0f;
                    currentFrame = (currentFrame + 1) % TotalFrames; // Use all 11 frames for walking animation
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
                    0, // Start at top of goblin.png (Y=0)
                    FrameWidth,
                    FrameHeight
                );
                
                SpriteEffects effects = facingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(texture, position, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteScale, effects, 0f);
            }
        }
    }
}
