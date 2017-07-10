using CarsAndPitsWPF2.Classes.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    struct CPVector
    {
        public PointLatLng coordinate;
        public long absoluteTime;
        public double X, Y, Z;
        public double length;

        public CPVector(PointLatLng coordinate, long absoluteTime, double X, double Y, double Z)
        {
            this.coordinate = coordinate;
            this.absoluteTime = absoluteTime;
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            length = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
    }
}
