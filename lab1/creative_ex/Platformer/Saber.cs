using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Platformer
{
    public class Saber
    {
        private Texture2D texture;
        private Player player;
        private bool isAttacking;
        private float attackTimer;
        private const float AttackDuration = 0.3f; // Attack lasts 0.3 seconds
        private const int AttackDamage = 25;
        private const int AttackRange = 100; // Increased range for larger sprites
        private const float SpriteScale = 2.0f; // Scale sprites to 2x their original size
        
        // Animation variables
        private int currentFrame;
        private float frameTimer;
        private const float FrameTime = 0.05f; // 50ms per frame
        private const int TotalFrames = 6; // Assuming 6 frames in the slash sprite
        private const int FrameWidth = 64; // Assuming each frame is 64x64
        private const int FrameHeight = 64;
        
        private KeyboardState previousKeyboardState;
        
        public bool IsAttacking => isAttacking;
        public Rectangle AttackBounds
        {
            get
            {
                if (!isAttacking) return Rectangle.Empty;
                
                Vector2 playerCenter = player.Position + new Vector2(32, 32); // Center of scaled 64x64 player
                return new Rectangle(
                    (int)(playerCenter.X - AttackRange / 2),
                    (int)(playerCenter.Y - AttackRange / 2),
                    AttackRange,
                    AttackRange
                );
            }
        }
        
        public Saber(Texture2D saberTexture, Player player)
        {
            this.texture = saberTexture;
            this.player = player;
            this.isAttacking = false;
            this.attackTimer = 0f;
            this.currentFrame = 0;
            this.frameTimer = 0f;
            this.previousKeyboardState = Keyboard.GetState();
        }
        
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState currentKeyboardState = Keyboard.GetState();
            
            // Check for attack input (X key)
            if (currentKeyboardState.IsKeyDown(Keys.X) && !previousKeyboardState.IsKeyDown(Keys.X) && !isAttacking)
            {
                StartAttack();
            }
            
            // Update attack
            if (isAttacking)
            {
                attackTimer += deltaTime;
                frameTimer += deltaTime;
                
                // Update animation frame
                if (frameTimer >= FrameTime)
                {
                    frameTimer = 0f;
                    currentFrame++;
                    
                    if (currentFrame >= TotalFrames)
                    {
                        currentFrame = 0;
                    }
                }
                
                // End attack
                if (attackTimer >= AttackDuration)
                {
                    EndAttack();
                }
            }
            
            previousKeyboardState = currentKeyboardState;
        }
        
        private void StartAttack()
        {
            isAttacking = true;
            attackTimer = 0f;
            currentFrame = 0;
            frameTimer = 0f;
        }
        
        private void EndAttack()
        {
            isAttacking = false;
            attackTimer = 0f;
            currentFrame = 0;
            frameTimer = 0f;
        }
        
        public bool CheckCollision(Rectangle enemyBounds)
        {
            if (!isAttacking) return false;
            return AttackBounds.Intersects(enemyBounds);
        }
        
        public int GetDamage()
        {
            return AttackDamage;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            if (isAttacking && texture != null)
            {
                Vector2 playerCenter = player.Position + new Vector2(32, 32); // Center of scaled 64x64 player
                Vector2 saberPosition = playerCenter - new Vector2((FrameWidth * SpriteScale) / 2, (FrameHeight * SpriteScale) / 2);
                
                Rectangle sourceRectangle = new Rectangle(
                    currentFrame * FrameWidth,
                    0,
                    FrameWidth,
                    FrameHeight
                );
                
                spriteBatch.Draw(texture, saberPosition, sourceRectangle, Color.White, 0f, Vector2.Zero, SpriteScale, SpriteEffects.None, 0f);
            }
        }
    }
}
