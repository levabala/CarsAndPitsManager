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

        public CPVector(CPVectorAbs vectorAbs)
        {
            X = vectorAbs.X;
            Y = vectorAbs.Y;
            Z = vectorAbs.Z;
            length = vectorAbs.length;
        }
    }

    public struct CPVectorAbs
    {        
        public long absoluteTime;
        public double X, Y, Z;
        public double length;

        public CPVectorAbs(long absoluteTime, double X, double Y, double Z)
        {            
            this.absoluteTime = absoluteTime;
            this.X = X;
            this.Y = Y;
            this.Z = Z;

            length = Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
        }

        public static CPVectorAbs[] fromArray(DataTuplya[] array, long absoluteTime)
        {
            CPVectorAbs[] output = new CPVectorAbs[array.Length];
            Parallel.For(0, array.Length, i =>
            {
                output[i] = new CPVectorAbs(absoluteTime + array[i].timeOffset, array[i].values[0], array[i].values[1], array[i].values[2]);
            });
            return output;
        }
    }    
}
