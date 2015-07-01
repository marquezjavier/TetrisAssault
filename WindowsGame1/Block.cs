using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace TetrisAssault
{
    class Block : Sprite
    {
        private bool dying;
        private bool falling;
        private int color; //0 = red , 1 = blue, 2 = green, 3 = purple, 4 = teal , 5 = grey

        //Fades Dying Block
        public bool isDying()
        {
            if (Alpha < 5)
            {
                //Makes Invisable
                dying = false;
                Visable = false;
            }

            if(dying == false)
                return false;
            else
            {
                Alpha -= 5;
                return true;
            }
        }

        //Sets Dying State
        public void setDying(bool newDying)
        {
            dying = newDying;
        }

        //Falling Block
        public bool isFalling
        {
            set{ falling = value; }
            get { return falling; }
        }

        public int Color
        {
            set { color = value; }
            get { return color; }
        }

        public Block()
        {
            this.Velocity = new Vector2(-5, 0);
            this.Size = new Vector2(40, 40);
            falling = false;
            dying = false;
        }

    }
}
