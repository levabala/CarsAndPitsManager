using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsAndPitsWPF2.Classes.Other
{
    struct GeoVector
    {
        public PointLatLng startP, endP;
        public double widthLng, heightLat, lengthMeters;

        public GeoVector(PointLatLng start, PointLatLng end)
        {
            startP = start;
            endP = end;
            widthLng = end.Lng - start.Lng;
            heightLat = end.Lat - start.Lat;
            lengthMeters = Math.Sqrt(widthLng * widthLng + heightLat * heightLat);
        }

        public GeoVector(PointLatLng start, double widthLng, double heightLat)
        {
            startP = start;
            endP = new PointLatLng(start.Lat + heightLat, start.Lng + widthLng);
            this.widthLng = widthLng;
            this.heightLat = heightLat;
            lengthMeters = Math.Sqrt(widthLng * widthLng + heightLat * heightLat);
        }
    }
}
