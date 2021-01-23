using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HideAndSeek
{
    
    abstract class Entity
    {
        public string name;
        public int posY, posX;
        public Image sprite;
        public int width, height;

    }
    class Biom : Entity
    {

        public Biom(Image biom)
        {
            this.sprite = biom;
            this.width = Form1.TILE_SIZE;
            this.height = Form1.TILE_SIZE;
        }
    }
    class Coin : Entity
    {
        
        public Coin()
        {
            this.sprite = Image.FromFile(@"..\..\Images\blueberryBush.png");
        }
    }
}
