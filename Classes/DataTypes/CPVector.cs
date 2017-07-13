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
        public PointGeo startP;
        public double dLat, dLng, dAlt;

        public CPVectorAbsGeo(double X, double Y, double Z, 
            long absoluteTime, PointGeo startP, double dLat, double dLng, double dAlt) : base(X,Y,Z,absoluteTime)
        {
            this.startP = startP;
            this.dLat = dLat;
            this.dLng = dLng;
            this.dAlt = dAlt;
        }

        public static CPVectorAbsGeo[] fromArray(DataTuplyaGeo[] array, long absoluteTime)
        {
            CPVectorAbsGeo[] output = new CPVectorAbsGeo[array.Length];

            output[0] = new CPVectorAbsGeo(
                    array[0].values[0], array[0].values[1],
                    array[0].values[2], absoluteTime + array[0].timeOffset,
                    (PointGeo)array[0].coordinate, 0, 0, 0);

            Parallel.For(1, array.Length, i =>
            {
                double dLat = array[i].coordinate.Latitude - array[i - 1].coordinate.Latitude;
                double dLng = array[i].coordinate.Longitude - array[i - 1].coordinate.Longitude;
                double dAlt = array[i].coordinate.Altitude - array[i - 1].coordinate.Altitude;
                output[i] = new CPVectorAbsGeo(
                    array[i].values[0], array[i].values[1], 
                    array[i].values[2], absoluteTime + array[i].timeOffset,
                    (PointGeo)array[i].coordinate, dLat, dLng, dAlt);
            });
            return output;
        }
    }
}
