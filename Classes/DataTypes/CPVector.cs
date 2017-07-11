using CarsAndPitsWPF2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    public struct CPVector
    {
        public PointLatLng coordinate;        
        public double X, Y, Z;
        public double length;        

        public CPVector(PointLatLng coordinate, double X, double Y, double Z)
        {
            this.coordinate = coordinate;            
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            length = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }
    }

    public struct CPVectorAbs
    {
        public PointLatLng coordinate;
        public long absoluteTime;
        public double X, Y, Z;
        public double length;

        public CPVectorAbs(PointLatLng coordinate, long absoluteTime, double X, double Y, double Z)
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
