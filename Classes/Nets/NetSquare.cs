using CarsAndPitsWPF2.Classes.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.Nets
{
    class NetSquare
    {
        public readonly int level;
        public readonly double lat;
        public readonly double lng;
        public readonly int[] path;

        public double intensity;
        public NetSquare[] children;
        public List<CPVector> vectors = new List<CPVector>();

        public NetSquare(double lat, double lng, int level, double intensity)
        {
            this.level = level;
            this.lat = lat;
            this.lng = lng;
            this.intensity = intensity;
            path = new int[0];
            children = new NetSquare[4];
        }

        public NetSquare(NetSquare parent, int index, double intensity)
        {
            level = parent.level + 1;
            lat = parent.lat + ((index > 1) ? 180 / Math.Pow(2, parent.level) / 2 : 0);
            lng = parent.lng + ((index == 1 || index == 3) ? 360 / Math.Pow(2, parent.level) / 2 : 0);
            children = new NetSquare[4];

            //path remembering
            path = new int[parent.path.Length + 1];
            parent.path.CopyTo(path, 0);
            path[path.Length - 1] = index;

            this.intensity = intensity;
        }        
    }
}
