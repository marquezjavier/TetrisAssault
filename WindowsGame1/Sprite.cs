using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TetrisAssault
{
    class Sprite
    {

        private Vector2 position;
        private Vector2 velocity;
        private Vector2 size;
        private bool visable = true;
        private byte alpha = 255;

        public byte Alpha
        {
            set { alpha = value; }
            get { return alpha; }
        }
        public Vector2 Velocity
        {
            set { velocity = value; }
            get { return velocity; }
        }

        public bool Visable
        {
            set { visable = value; }
            get { return visable; }
        }

        public Vector2 Position 
        {
            set { position = value; }
            get { return position; }
        }

        public Vector2 Size
        {
            set { size = value; }
            get { return size; }
        }

        public Texture2D texture { get; set; }
        

        public Sprite(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, Vector2 newVelocity)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            velocity = newVelocity;
        }
        public Sprite()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, new Color(255, 255, 255, alpha));
        }

        public void Move()
        {
           
        }

        //Checks Collision
        public bool Collides(Sprite otherSprite)
        {
            if (this.position.X + this.size.X > otherSprite.position.X &&
                this.position.X < otherSprite.position.X + otherSprite.size.X &&
                this.position.Y + this.size.Y > otherSprite.position.Y &&
                this.position.Y < otherSprite.position.Y + otherSprite.size.Y)
                return true;
            else
                return false;
        }
        public void Move(int direction, int intensity)
        {
            //Down
            if (direction == 0)
                this.position.Y += intensity;
            //Up
            if (direction == 1)
                this.position.Y -= intensity;
            //Right
            if (direction == 2)
                this.position.X += intensity;
            //Left
            if (direction == 3)
                this.position.X -= intensity;
        }
    }
}
