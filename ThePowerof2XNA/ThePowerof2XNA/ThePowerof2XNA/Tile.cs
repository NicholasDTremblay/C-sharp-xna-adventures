using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ThePowerof2XNA
{
    public class Tile
    {
        //public int points = 50;
        //public int velocity = 10;
        public int myX;
        public int myY;
        public int colourNum;

        public Boolean flagged; // flagged for death

        public int animationFrame;

        public Texture2D myImage;

        public Tile(int x, int y, Texture2D theImage, int colour)
        {
            flagged = false;

            myX = x;
            myY = y;
            animationFrame = 0;

            myImage = theImage;

            colourNum = colour;

        }

        public void collide(Tile otherTile, int gravityDirection)// 0 = down 1 = up 2 = left 3 = right
        {
            switch (gravityDirection)
            {
                case 0://down
                    if (myY + myImage.Height > otherTile.myY)
                    {
                        myY = otherTile.myY - myImage.Height;
                    }
                    if (myY + myImage.Height > 400) // hardcoded for now
                    {
                        myY = 400 - myImage.Height;
                    }
                    break;
                case 1://up
                    if (myY < otherTile.myY + otherTile.myImage.Height)
                    {
                        myY = otherTile.myY + otherTile.myImage.Height;
                    }
                    if (myY < 0) // hardcoded for now
                    {
                        myY = 0;
                    }
                    break;
                case 2://left
                    if (myX < otherTile.myX + otherTile.myImage.Width)
                    {
                        myX = otherTile.myX + otherTile.myImage.Width;
                    }
                    if (myX < 0) // hardcoded for now
                    {
                        myX = 0;
                    }
                    break;
                case 3://right
                    if (myX + myImage.Width > otherTile.myX)
                    {
                        myX = otherTile.myX - myImage.Width;
                    }
                    if (myX + myImage.Width > 400) // hardcoded for now
                    {
                        myX = 400 - myImage.Width;
                    }
                    break;
                default:
                    break;
            }
        }

        public Boolean matchUp(Tile otherTile)
        {
            if (this.colourNum == otherTile.colourNum)
            {
                return true;
            }

            return false;
        }

        public void animate()
        {
            animationFrame++;
        }
    }
}
