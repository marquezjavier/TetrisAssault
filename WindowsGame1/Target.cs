using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TetrisAssault
{
    class Target : Sprite
    {
        //Tracks targets
        private int currentTargetX = 0;
        private int currentTargetY = 0;

        public int CurrentTargetX
        {
            set { currentTargetX = value; }
            get { return currentTargetX; }
        }
        public int CurrentTargetY
        {
            set { currentTargetY = value; }
            get { return currentTargetY; }
        }

        public Target(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, Vector2 newVelocity)
        {
            this.texture = newTexture;
            this.Position = newPosition;
            this.Size = newSize;
            this.Velocity = newVelocity;
        }
    }
}
