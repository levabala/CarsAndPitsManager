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
        public double X, Y, Z;
        public double length;        

        public CPVector(double X, double Y, double Z)
        {            
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            length = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }

        public static CPVector[] fromArray(DataTuplya[] array)
        {
            CPVector[] output = new CPVector[array.Length];
            Parallel.For(0, array.Length-1, i =>
            {
                output[i] = new CPVector(array[i].values[0], array[i].values[1], array[i].values[2]);
            });
            return output;
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
