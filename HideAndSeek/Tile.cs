using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HideAndSeek
{
    class Tile
    {
        Entity biom;
        int spriteId;
        public List<Entity> entities = new List<Entity>();


        public Tile(Entity biom)
        {
            this.biom = biom;
            this.spriteId = spriteId;
        }

        public Entity getTexture()
        {
            return this.biom;
        }
    }
}
