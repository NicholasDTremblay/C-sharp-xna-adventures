using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePowerof2XNA
{
    public class placement
    {
        public Tile theTile;
        public Boolean occupied;

        public placement()
        {
            theTile = null;
            occupied = false;
        }

        public void emptyOut()
        {
            theTile = null;
            occupied = false;
        }
    }
}
