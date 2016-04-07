using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galaxy
{
    class SpaceObject
    {
        public int x;
        public int y;
    }

    class Star: SpaceObject
    {
        public Color color;
        public int size;
        public List<Planet> planets;
    }

    class Planet:SpaceObject
    {
        public Color color;
        public int size;
        public List<Satellit> satellits;
        public double angle;
        public double dangle;

    }
    class Satellit:SpaceObject
    {
        public Color color;
        public int size;
        public double angle;
        public double dangle;
    }
}
