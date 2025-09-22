using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Platformer
{
    public class Level
    {
        private List<Rectangle> platforms;
        private Rectangle groundLevel;
        private Texture2D pixelTexture;
        private GraphicsDevice graphics;
        
        public List<Rectangle> Platforms => platforms;
        public Rectangle Ground => groundLevel;
        
        public Level(GraphicsDevice graphics, Texture2D pixelTexture)
        {
            this.graphics = graphics;
            this.pixelTexture = pixelTexture;
            platforms = new List<Rectangle>();
            CreateLevel();
        }
        
        private void CreateLevel()
        {
            int screenWidth = graphics.Viewport.Width;
            int screenHeight = graphics.Viewport.Height;
            
            // Create ground level
            groundLevel = new Rectangle(0, screenHeight - 50, screenWidth, 50);
            
            // Create platforms - designing a simple level
            platforms.Add(new Rectangle(200, screenHeight - 150, 150, 20)); // Platform 1
            platforms.Add(new Rectangle(400, screenHeight - 200, 120, 20)); // Platform 2
            platforms.Add(new Rectangle(100, screenHeight - 250, 100, 20)); // Platform 3
            platforms.Add(new Rectangle(550, screenHeight - 300, 150, 20)); // Platform 4
            platforms.Add(new Rectangle(300, screenHeight - 350, 200, 20)); // Platform 5 (top)
            
            // Add some side platforms for variety
            platforms.Add(new Rectangle(50, screenHeight - 180, 80, 20));   // Left side
            platforms.Add(new Rectangle(650, screenHeight - 160, 100, 20)); // Right side
        }
        
        public bool CheckCollision(Rectangle entityBounds, Vector2 velocity, out Vector2 newPosition, out Vector2 newVelocity, out bool isOnGround)
        {
            newPosition = new Vector2(entityBounds.X, entityBounds.Y);
            newVelocity = velocity;
            isOnGround = false;
            bool collided = false;
            
            // Create a new rectangle for the entity's next position
            Rectangle nextBounds = new Rectangle(
                (int)(entityBounds.X + velocity.X),
                (int)(entityBounds.Y + velocity.Y),
                entityBounds.Width,
                entityBounds.Height
            );
            
            // Check collision with ground
            if (CheckPlatformCollision(groundLevel, entityBounds, velocity, ref newPosition, ref newVelocity, ref isOnGround))
            {
                collided = true;
            }
            
            // Check collision with all platforms
            foreach (var platform in platforms)
            {
                if (CheckPlatformCollision(platform, entityBounds, velocity, ref newPosition, ref newVelocity, ref isOnGround))
                {
                    collided = true;
                }
            }
            
            return collided;
        }
        
        private bool CheckPlatformCollision(Rectangle platform, Rectangle entityBounds, Vector2 velocity, ref Vector2 newPosition, ref Vector2 newVelocity, ref bool isOnGround)
        {
            Rectangle nextBounds = new Rectangle(
                (int)(entityBounds.X + velocity.X),
                (int)(entityBounds.Y + velocity.Y),
                entityBounds.Width,
                entityBounds.Height
            );
            
            if (nextBounds.Intersects(platform))
            {
                // Calculate overlap areas to determine best collision resolution
                Rectangle overlap = Rectangle.Intersect(nextBounds, platform);
                
                // Determine collision direction based on velocity and overlap
                bool landingOnTop = velocity.Y > 0 && entityBounds.Bottom <= platform.Top + 10;
                bool hittingFromBelow = velocity.Y < 0 && entityBounds.Top >= platform.Bottom - 10;
                bool hittingFromSide = Math.Abs(velocity.X) > Math.Abs(velocity.Y);
                
                // Prioritize vertical collisions (landing/hitting ceiling)
                if (landingOnTop)
                {
                    newPosition.Y = platform.Top - entityBounds.Height;
                    newVelocity.Y = 0;
                    isOnGround = true;
                    return true;
                }
                else if (hittingFromBelow)
                {
                    newPosition.Y = platform.Bottom;
                    newVelocity.Y = 0;
                    return true;
                }
                // Handle side collisions with better resolution
                else if (hittingFromSide && velocity.X != 0)
                {
                    if (velocity.X > 0) // Moving right
                    {
                        newPosition.X = platform.Left - entityBounds.Width - 1; // Add 1 pixel buffer
                    }
                    else // Moving left
                    {
                        newPosition.X = platform.Right + 1; // Add 1 pixel buffer
                    }
                    newVelocity.X = 0;
                    return true;
                }
            }
            
            return false;
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw ground
            spriteBatch.Draw(pixelTexture, groundLevel, Color.Green);
            
            // Draw platforms
            foreach (var platform in platforms)
            {
                spriteBatch.Draw(pixelTexture, platform, Color.Brown);
            }
        }
    }
}
