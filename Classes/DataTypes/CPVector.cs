using CarsAndPitsWPF2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.DataTypes
{
    public class CPVector
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

    public class CPVectorAbs : CPVector
    {        
        public long absoluteTime;        

        public CPVectorAbs(double X, double Y, double Z, long absoluteTime) : base (X, Y, Z)
        {            
            this.absoluteTime = absoluteTime;            
        }

        public static CPVectorAbs[] fromArray(DataTuplya[] array, long absoluteTime)
        {
            CPVectorAbs[] output = new CPVectorAbs[array.Length];
            Parallel.For(0, array.Length, i =>
            {
                output[i] = new CPVectorAbs(array[i].values[0], array[i].values[1], array[i].values[2], absoluteTime + array[i].timeOffset);
            });
            return output;
        }
    }    

    public class CPVectorAbsGeo : CPVectorAbs
    {
        public PointLatLng startP, endP;
        public double widthLng, heightLat, lengthMeters;

        public CPVectorAbsGeo(double X, double Y, double Z, 
            long absoluteTime, PointLatLng startP, PointLatLng endP) : base(X,Y,Z,absoluteTime)
        {
            this.startP = startP;
            this.endP = endP;
            widthLng = endP.Lng - startP.Lng;
            heightLat = endP.Lat - startP.Lat;
            lengthMeters = Math.Sqrt(widthLng * widthLng + heightLat * heightLat);
        }
    }
}
