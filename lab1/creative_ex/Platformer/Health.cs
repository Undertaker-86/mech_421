using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Platformer
{
    public class Health
    {
        private int currentHealth;
        private int maxHealth;
        
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;
        public float HealthPercentage => (float)currentHealth / maxHealth;
        
        public Health(int maxHealth)
        {
            this.maxHealth = maxHealth;
            this.currentHealth = maxHealth;
        }
        
        public void TakeDamage(int damage)
        {
            currentHealth = Math.Max(0, currentHealth - damage);
        }
        
        public void Heal(int healAmount)
        {
            currentHealth = Math.Min(maxHealth, currentHealth + healAmount);
        }
        
        public void Reset()
        {
            currentHealth = maxHealth;
        }
    }
    
    public class HealthBar
    {
        private Texture2D pixelTexture;
        private Vector2 position;
        private Vector2 size;
        private Color backgroundColor;
        private Color healthColor;
        
        public HealthBar(Texture2D pixelTexture, Vector2 position, Vector2 size)
        {
            this.pixelTexture = pixelTexture;
            this.position = position;
            this.size = size;
            this.backgroundColor = Color.Red;
            this.healthColor = Color.Green;
        }
        
        public void Draw(SpriteBatch spriteBatch, Health health)
        {
            // Draw background (red)
            Rectangle backgroundRect = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            spriteBatch.Draw(pixelTexture, backgroundRect, backgroundColor);
            
            // Draw health (green)
            int healthWidth = (int)(size.X * health.HealthPercentage);
            if (healthWidth > 0)
            {
                Rectangle healthRect = new Rectangle((int)position.X, (int)position.Y, healthWidth, (int)size.Y);
                spriteBatch.Draw(pixelTexture, healthRect, healthColor);
            }
            
            // Draw border (black)
            Rectangle topBorder = new Rectangle((int)position.X, (int)position.Y, (int)size.X, 1);
            Rectangle bottomBorder = new Rectangle((int)position.X, (int)position.Y + (int)size.Y - 1, (int)size.X, 1);
            Rectangle leftBorder = new Rectangle((int)position.X, (int)position.Y, 1, (int)size.Y);
            Rectangle rightBorder = new Rectangle((int)position.X + (int)size.X - 1, (int)position.Y, 1, (int)size.Y);
            
            spriteBatch.Draw(pixelTexture, topBorder, Color.Black);
            spriteBatch.Draw(pixelTexture, bottomBorder, Color.Black);
            spriteBatch.Draw(pixelTexture, leftBorder, Color.Black);
            spriteBatch.Draw(pixelTexture, rightBorder, Color.Black);
        }
    }
}
