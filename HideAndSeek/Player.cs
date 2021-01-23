using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HideAndSeek
{
    class Player : Entity
    {
        bool hider;
        bool onLeft;
    
        public int movingX, movingY = 0;
        public int distanceTraveled = 0;
        public int gamesWon = 0;

        public Player(bool hider, bool onLeft, int x, int y)
        {
            if (onLeft)
            {
                this.sprite = Image.FromFile(@"..\..\Images\BBB_idle_back.png");
            }
            else
            {
                this.sprite = Image.FromFile(@"..\..\Images\BBB_idle_front.png");
            }
            this.width = Form1.TILE_SIZE / 4;
            this.height = Form1.TILE_SIZE / 4;
            this.hider = hider;
            this.posX = x;
            this.posY = y;
            this.onLeft = onLeft;

        }

        public bool getOnLeft()
        {
            return this.onLeft;
        }

        public bool getHider()
        {
            return this.hider;
        }

        public int getX()
        {
            return this.posX;
        }

        public int getY()
        {
            return this.posY;
        }
        public void setX(int x)
        {
            this.posX = x;
        }

        public void setY(int y)
        {
            this.posY = y;
        }


    }
}
